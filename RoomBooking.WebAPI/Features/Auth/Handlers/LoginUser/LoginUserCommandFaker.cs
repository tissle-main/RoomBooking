using Bogus;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.LoginUser;

public static class LoginUserCommandFaker
{
    extension(Faker<LoginUserCommand> thisFaker)
    {
        public Faker<LoginUserCommand> Valid()
        {
            return thisFaker.CustomInstantiator(gen =>
            {
                string email = gen.Internet.ExampleEmail(gen.Person.FirstName, gen.Person.LastName);
                string password = gen.Internet.Password(length: AuthConstraints.PasswordMinLength, regexPattern: "[A-Za-z0-9]", prefix: "Aa0");
                return new LoginUserCommand(email, password);
            });
        }
        public Faker<LoginUserCommand> WithEmail(string email)
        {
            return thisFaker.RuleFor(c => c.Email, g => email);
        }
        public Faker<LoginUserCommand> WithPassword(string password)
        {
            return thisFaker.RuleFor(c => c.Password, g => password);
        }
        public Faker<LoginUserCommand> WithInvalidEmail()
        {
            return thisFaker.RuleFor(
                c => c.Email,
                g => g.Internet.ExampleEmail(g.Person.FirstName, g.Person.LastName).Replace("@", "")
            );
        }
        public Faker<LoginUserCommand> WithTooShortPassword()
        {
            return thisFaker.RuleFor(
                c => c.Password,
                g => g.Internet.Password(length: AuthConstraints.PasswordMinLength - 1, regexPattern: "[A-Za-z0-9]", prefix: "Aa0")
            );
        }
        public Faker<LoginUserCommand> WithPasswordWithoutUppercaseLetters()
        {
            return thisFaker.RuleFor(
                c => c.Password,
                g => g.Internet.Password(length: AuthConstraints.PasswordMinLength, regexPattern: "[a-z0-9]", prefix: "a0")
            );
        }
        public Faker<LoginUserCommand> WithPasswordWithoutLowercaseLetters()
        {
            return thisFaker.RuleFor(
                c => c.Password,
                g => g.Internet.Password(length: AuthConstraints.PasswordMinLength, regexPattern: "[A-Z0-9]", prefix: "A0")
            );
        }
        public Faker<LoginUserCommand> WithPasswordWithoutDigits()
        {
            return thisFaker.RuleFor(
                c => c.Password,
                g => g.Internet.Password(length: AuthConstraints.PasswordMinLength, regexPattern: "[A-Za-z]", prefix: "Aa")
            );
        }
    }
}