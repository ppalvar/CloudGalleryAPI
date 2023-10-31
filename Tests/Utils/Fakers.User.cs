using System;
using System.Collections.Generic;
using Application.Models;
using Bogus;

namespace Tests.Utils;

public partial class Fakers
{
    public class UserFaker
    {
        private readonly Faker<Photo> _fakerPhoto = new Faker<Photo>();
        private readonly Faker<User> _fakerUser = new Faker<User>();

        public UserFaker()
        {
            _fakerUser.RuleFor(x => x.Email, x => x.Person.Email)
                  .RuleFor(x => x.Username, x => x.Person.UserName)
                  .RuleFor(x => x.IsBanned, x => false)
                  .RuleFor(x => x.IsEmailVerified, x => true)
                  .RuleFor(x => x.IsBanned, x => false)
                  .RuleFor(x => x.PasswordHash, x => GetRandomBytes())
                  .RuleFor(x => x.PasswordSalt, x => GetRandomBytes())
                  .RuleFor(x => x.Photos, x => default!);

            _fakerPhoto.RuleFor(x => x.Title, x => x.Hacker.Phrase())
                .RuleFor(x => x.Description, x => x.Lorem.Lines(1))
                .RuleFor(x => x.Width, x => new Random().Next())
                .RuleFor(x => x.Heigth, x => new Random().Next())
                .RuleFor(x => x.Owner, x => default!);
        }

        public User GetUser(List<Photo>? photos=null)
        {
            var user = _fakerUser.Generate();

            photos ??= _fakerPhoto.Generate(new Random().Next(1,10));
            user.Photos = photos;

            return user;
        }

        private byte[] GetRandomBytes(int count = 10)
        {
            byte[] bytes = new byte[count];

            var rnd = new Random();
            rnd.NextBytes(bytes);

            return bytes;
        }
    }
}