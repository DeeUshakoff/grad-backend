namespace AuthService.Helpers
{
    public interface IEncryptableEntity
    {
        public void EncryptSelf(IEncryptionService encryptionService);
    }
}
