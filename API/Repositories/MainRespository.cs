using System.Threading.Tasks;
using API.Data;
using API.Interface;
using AutoMapper;

namespace API.Repositories
{
    public class MainRespository : IRespositories
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MainRespository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public IAppUserRespository AppUserRespository => new AppUserRespository(_context, _mapper);
        public IMessageRepository MessageRepository => new MessageRespository(_context, _mapper);
        public IUserLikesRespository UserLikesRespository => new UserLikesRespository(_context);

        public async Task<bool> Complete()
        {
            return await  _context.SaveChangesAsync() > 0;   
        }

        public bool HasChanges()
        {
           return _context.ChangeTracker.HasChanges();     
        }
    }
}