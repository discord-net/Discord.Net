using Discord.API.Client.Rest;
using System;
using System.IO;
using System.Threading.Tasks;
using APIUser = Discord.API.Client.User;

namespace Discord
{
    public sealed class Profile
    {
        internal DiscordClient Client { get; }

        /// <summary> Gets the unique identifier for this user. </summary>
        public ulong Id { get; private set; }
		/// <summary> Gets the email for this user. </summary>
		public string Email { get; private set; }
		/// <summary> Gets if the email for this user has been verified. </summary>
		public bool? IsVerified { get; private set; }

        internal Profile(DiscordClient client)
        {
            Client = client;
        }

        internal void Update(APIUser model)
        {
            Id = model.Id;
            Email = model.Email;
            IsVerified = model.IsVerified;
        }

        public async Task Edit(string currentPassword = "",
            string username = null, string email = null, string password = null,
            Stream avatar = null, ImageType avatarType = ImageType.Png)
        {
            if (currentPassword == null) throw new ArgumentNullException(nameof(currentPassword));

            var request = new UpdateProfileRequest()
            {
                CurrentPassword = currentPassword,
                Email = email ?? Email,
                Password = password,
                Username = username ?? Client.PrivateUser.Name,
                AvatarBase64 = avatar.Base64(avatarType, Client.PrivateUser.AvatarId)
            };

            await Client.ClientAPI.Send(request).ConfigureAwait(false);

            if (password != null)
            {
                var loginRequest = new LoginRequest()
                {
                    Email = Email,
                    Password = password
                };
                var loginResponse = await Client.ClientAPI.Send(loginRequest).ConfigureAwait(false);
                Client.ClientAPI.Token = loginResponse.Token;
                Client.GatewaySocket.Token = loginResponse.Token;
            }
        }

        public override bool Equals(object obj) 
            => (obj is Profile && (obj as Profile).Id == Id) || (obj is User && (obj as User).Id == Id);
        public override int GetHashCode() => unchecked(Id.GetHashCode() + 2061);
        public override string ToString() => Id.ToIdString();
    }
}
