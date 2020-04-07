using Domain;
using Domain.IRepositories;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public MessengerContext _db { get; private set; }

        private Lazy<IUserRepository> userRepository;
        private Lazy<IConversationInfoRepository> photoRepository;
        private Lazy<IMessageRepository> messageRepository;
        private Lazy<IConversationRepository> chatRepository;
        private Lazy<IBlockedUserRepository> blockeduserRepository;
        private Lazy<IUserConversationRepository> userConversationRepository;

        public IUserConversationRepository UserConversationRepository
        {
            get
            {
                return this.userConversationRepository.Value;
            }
        }
        public IConversationRepository ConversationRepository
        {
            get
            {
                return this.chatRepository.Value;
            }
        }
        public IBlockedUserRepository BlockedUserRepository
        {
            get
            {
                return this.blockeduserRepository.Value;
            }
        }
        public IConversationInfoRepository ConversationInfoRepository
        {
            get
            {
                return this.photoRepository.Value;
            }
        }
        public IMessageRepository MessageRepository
        {
            get
            {
                return this.messageRepository.Value;
            }
        }
        public IUserRepository UserRepository
        {
            get
            {
                return this.userRepository.Value;
            }
        }


        public UnitOfWork(MessengerContext db)
        {
            _db = db;

            this.userRepository = new Lazy<IUserRepository>(() => new UserRepository(_db));

            this.chatRepository = new Lazy<IConversationRepository>(() => new ConversationRepository(_db));

            this.blockeduserRepository = new Lazy<IBlockedUserRepository>(() => new BlockedUserRepository(_db));

            this.messageRepository = new Lazy<IMessageRepository>(() => new MessageRepository(_db));

            this.photoRepository = new Lazy<IConversationInfoRepository>(() => new PhotoRepository(_db));

            this.userConversationRepository = new Lazy<IUserConversationRepository>(() => new UserConversationRepository(_db));

        }

        public async Task Commit()
        {
            await _db.SaveChangesAsync();
        }
    }
}
