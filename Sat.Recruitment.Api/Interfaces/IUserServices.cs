using System.IO;

namespace Sat.Recruitment.Api.Interfaces
{
    public interface IUserServices
    {
        public StreamReader ReadUsersFromFile();
    }
}