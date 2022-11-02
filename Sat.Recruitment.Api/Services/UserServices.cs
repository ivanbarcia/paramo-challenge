using System.IO;
using Sat.Recruitment.Api.Interfaces;

namespace Sat.Recruitment.Api.Services
{
    public class UserServices : IUserServices
    {
        public StreamReader ReadUsersFromFile()
        {
            var path = Directory.GetCurrentDirectory() + "/Files/Users.txt";

            FileStream fileStream = new FileStream(path, FileMode.Open);

            StreamReader reader = new StreamReader(fileStream);
            return reader;
        }
    }
}