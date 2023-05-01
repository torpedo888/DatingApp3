using System;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public Task<PagedList<MessageDto>> GetMessagesForUser()
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(x=>x.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch 
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.UserName
                    && u.RecipientDeleted == false), //ahol a cimzett a belogolt user..
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.UserName 
                    && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.UserName 
                    && u.RecipientDeleted == false && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUsername)
        {
            //mivel ez egy message thread, a user a sajat kuldott es a fogadott uzeneteket is latja egyben ezert
            //van a queryben az or || hogy mind a kettot kiszurjuk, ahol o a sender vagy o a recipient...

            var messages = await _context.Messages
             .Include(u=>u.Sender).ThenInclude(p=>p.Photos)
             .Include(u => u.Recipient).ThenInclude(p=>p.Photos)
             .Where(
                m=> m.RecipientUsername.ToLower() == currentUserName.ToLower()  && m.RecipientDeleted == false &&
                m.SenderUsername.ToLower() == recipientUsername.ToLower()||
                m.RecipientUsername.ToLower() == recipientUsername.ToLower() && m.SenderDeleted == false &&
                m.SenderUsername.ToLower() == currentUserName.ToLower()
             )
             .OrderBy(m => m.MessageSent)
             .ToListAsync();

             var unreadMessages = messages.Where(m=>m.DateRead == null && 
                    m.RecipientUsername.ToLower() == currentUserName.ToLower()).ToList();

             if(unreadMessages.Any())
             {
                // az amikor kilistazzuk az olvasatlan msg-eket azok olvasottak lesznek mikor megjelennek majd a tabon
                //igy azok dateread-je lesz a datetime.now

                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
             }

             return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}