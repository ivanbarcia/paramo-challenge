using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Sat.Recruitment.Api.Interfaces;
using Sat.Recruitment.Api.Models;

namespace Sat.Recruitment.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _services;

        public UserController(IUserServices services)
        {
            _services = services;
        }

        [HttpPost]
        [Route("/create-user")]
        public async Task<Result> CreateUser(User model)
        {
            try
            {
                const string errors = "";

                if (!ModelState.IsValid)
                {
                    return new Result
                    {
                        IsSuccess = false,
                        Errors = errors
                    };
                }

                model.Money = CalculateMoney(model);
                model.Email = NormalizeEmail(model.Email);
                var users = await ReadUsers();

                var isDuplicated = users.Exists(x =>
                    x.Email == model.Email ||
                    x.Phone == model.Phone ||
                    x.Name == model.Name ||
                    x.Address == model.Address);

                if (isDuplicated)
                {
                    Debug.WriteLine("The user is duplicated");

                    return new Result()
                    {
                        IsSuccess = false,
                        Errors = "The user is duplicated"
                    };
                }

                Debug.WriteLine("User Created");

                return new Result()
                {
                    IsSuccess = true,
                    Errors = "User Created"
                };
            }
            catch (Exception ex)
            {
                return new Result()
                {
                    IsSuccess = false,
                    Errors = ex.Message
                };
            }
        }
        
        private decimal CalculateMoney(User model)
        {
            decimal money = 0;

            switch (model.UserType)
            {
                case "Normal":
                {
                    if (model.Money > 100)
                    {
                        var percentage = Convert.ToDecimal(0.12);
                        //If new user is normal and has more than USD100
                        var gif = model.Money * percentage;
                        money = model.Money + gif;
                    }

                    if (model.Money < 100)
                    {
                        if (model.Money > 10)
                        {
                            var percentage = Convert.ToDecimal(0.8);
                            var gif = model.Money * percentage;
                            money = model.Money + gif;
                        }
                    }

                    break;
                }
                case "SuperUser":
                {
                    if (model.Money > 100)
                    {
                        var percentage = Convert.ToDecimal(0.20);
                        var gif = model.Money * percentage;
                        money = model.Money + gif;
                    }

                    break;
                }
                case "Premium":
                {
                    if (model.Money > 100)
                    {
                        var gif = model.Money * 2;
                        money = model.Money + gif;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return money;
        }

        private string NormalizeEmail(string email)
        {
            var aux = email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

            var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);

            aux[0] = atIndex < 0 ? aux[0].Replace(".", "") : aux[0].Replace(".", "").Remove(atIndex);

            return string.Join("@", new string[] { aux[0], aux[1] });
        }

        private async Task<List<User>> ReadUsers()
        {
            var users = new List<User>();
            var reader = _services.ReadUsersFromFile();
            while (reader.Peek() >= 0)
            {
                var line = reader.ReadLineAsync().Result;
                if (line == null) continue;
                
                var user = new User
                {
                    Name = line.Split(',')[0].ToString(),
                    Email = line.Split(',')[1].ToString(),
                    Phone = line.Split(',')[2].ToString(),
                    Address = line.Split(',')[3].ToString(),
                    UserType = line.Split(',')[4].ToString(),
                    Money = decimal.Parse(line.Split(',')[5].ToString()),
                };
                users.Add(user);
            }

            reader.Close();
            return users;
        }
    }
}