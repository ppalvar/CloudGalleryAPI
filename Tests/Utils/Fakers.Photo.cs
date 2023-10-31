namespace Tests.Utils;

using System;
using Application.Models;
using Bogus;

public static partial class Fakers
{
    public class PhotoFaker
    {
        private readonly Faker<Photo> _fakerPhoto = new Faker<Photo>();
        private readonly Faker<User> _fakerUser = new Faker<User>();

        public PhotoFaker()
        {
            _fakerPhoto.RuleFor(x => x.Title, x => x.Hacker.Phrase())
                .RuleFor(x => x.Description, x => x.Lorem.Lines(1))
                .RuleFor(x => x.Width, x => new Random().Next())
                .RuleFor(x => x.Heigth, x => new Random().Next())
                .RuleFor(x => x.Owner, x => default!);
            
            _fakerUser.RuleFor(x => x.Email, x => x.Person.Email)
                  .RuleFor(x => x.Username, x => x.Person.UserName)
                  .RuleFor(x => x.IsBanned, x => false)
                  .RuleFor(x => x.IsEmailVerified, x => true)
                  .RuleFor(x => x.IsBanned, x => false)
                  .RuleFor(x => x.PasswordHash, x => default!)
                  .RuleFor(x => x.PasswordSalt, x => default!);
        }

        public Photo GetPhoto(User user=null!) {
            var photo = _fakerPhoto.Generate();

            user ??= _fakerUser.Generate();
            photo.Owner = user;

            return photo;
        }
    }
}