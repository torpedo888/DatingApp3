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

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                    .Include(x => x.Connections)
                    .FirstOrDefaultAsync(x => x.Name == groupName);
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

            var query = _context.Messages
             .Where(
                m=> m.RecipientUsername.ToLower() == currentUserName.ToLower()  && m.RecipientDeleted == false &&
                m.SenderUsername.ToLower() == recipientUsername.ToLower()||
                m.RecipientUsername.ToLower() == recipientUsername.ToLower() && m.SenderDeleted == false &&
                m.SenderUsername.ToLower() == currentUserName.ToLower()
             )
             .OrderBy(m => m.MessageSent)
             .AsQueryable();

             var unreadMessages = query.Where(m=>m.DateRead == null && 
                    m.RecipientUsername.ToLower() == currentUserName.ToLower()).ToList();

             if(unreadMessages.Any())
             {
                // az amikor kilistazzuk az olvasatlan msg-eket azok olvasottak lesznek mikor megjelennek majd a tabon
                //igy azok dateread-je lesz a datetime.now

                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
             }

            // return _mapper.Map<IEnumerable<MessageDto>>(messages);
            return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

    }
}