using Newtonsoft.Json;
using APIUser = Discord.API.Client.User;

namespace Discord
{
    public sealed class Profile
    {
        /// <summary> Gets the unique identifier for this user. </summary>
        public ulong Id { get; private set; }
		/// <summary> Gets the email for this user. </summary>
		public string Email { get; private set; }
		/// <summary> Gets if the email for this user has been verified. </summary>
		public bool? IsVerified { get; private set; }

        internal Profile() { }

        internal void Update(APIUser model)
        {
            Id = model.Id;
            Email = model.Email;
            IsVerified = model.IsVerified;
        }

        public override bool Equals(object obj) 
            => (obj is Profile && (obj as Profile).Id == Id) || (obj is User && (obj as User).Id == Id);
        public override int GetHashCode() => unchecked(Id.GetHashCode() + 2061);
        public override string ToString() => Id.ToIdString();
    }
}
