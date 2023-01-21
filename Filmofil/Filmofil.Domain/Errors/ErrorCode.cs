namespace Filmofil.Domain.Errors
{
    public enum ErrorCode
    {
        InvalidArguments = 0,
        InvalidUserId = 1,
        UnacceptableContentType = 2,
        WrongOldPassword = 3,
        WrongEmail = 4,
        WrongPassword = 5,
        DuplicateEmail = 6,
        DuplicateUserName = 7,
        PasswordTooShort = 8,
        InvalidEmail = 9,
        EmailTaken = 10
    }
}
