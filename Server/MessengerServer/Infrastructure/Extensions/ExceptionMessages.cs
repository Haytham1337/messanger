namespace Infrastructure.Extensions
{
    public static class ExceptionMessages
    {
        public const string EmaisNotConfirmed = "Email is not confirmed!";
        public const string CredentialsNotValid = "Given credentials are not valid!";
        public const string UserNotExist = "Given user doesn`t exist!";
        public const string InvalidToken = "Given token is not valid!";
        public const string ConversationAlreadyExist = "Given conversation doesn`t exist!";
        public const string ConversationNotExist = "Given conversation doesn`t exist!";
        public const string NotHaveRigths = "Given user doesn`t have rigths for the action!";
        public const string InvalidConversationType = "Action cannot be done because of the invalid conversation type!";
        public const string NotMember = "Given user is not conversation member!";
        public const string AlreadyMember = "Given user is already conversation member!";
        public const string MessageInCorrect = "Given message is incorrect!";
        public const string UserAlreadyBlocked = "Given user is already blocked!";
        public const string UserAlreadyUnBlocked = "Given user is already unblocked!";
    }
}
