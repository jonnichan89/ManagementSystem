
namespace ManagementSystem.DataServices.Service
{
    public class MstUserService
    {
        //SharedResources _resShared;
        //public MstUserService(SharedResources resShared) { }
        public MstUserService() { }

        public List<ModelValidation> CanContinueAction(ServiceAction action, MstUser model)
        {
            List<ModelValidation> validations = new List<ModelValidation>();

            if (string.IsNullOrWhiteSpace(model.UserName))
                validations.Add(ModelValidation.FieldIsRequired(nameof(MstUser), nameof(model.UserName), model.UserName));

            if (action == ServiceAction.Delete)
            {
            }
            else
            {
                if (IsExists(action, model))
                    validations.Add(ModelValidation.PKAlreadyExists(nameof(MstUser), nameof(model.UserName), model.UserName));

            }

            return validations;
        }

        public IEnumerable<MstUser> GetMstUsers()
        {
            using (var context = new ManagementSystemContext())
            {
                var users = context.MstUsers.ToList();
                users.ForEach(u =>
                {
                    u.UserPassword = "";
                    u.Salt = "";
                });

                return users;
            }
        }

        public bool IsExists(ServiceAction action, MstUser model)
        {
            if (action == ServiceAction.Add || action == ServiceAction.Get)
                using (var context = new ManagementSystemContext())
                {
                    return context.MstUsers.Any(u => u.UserName == model.UserName);
                }

            return false;
        }

        public MstUser? GetMstUserByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));

            using (var context = new ManagementSystemContext())
            {
                MstUser? user = context.MstUsers.SingleOrDefault(u => u.UserName == userName);
                if (user != null)
                {
                    user.UserPassword = "";
                    user.Salt = "";
                }
                return user;
            }
        }

        public MstUser? CheckLogin(string userName, string password)
        {
            MstUser? user = null;
            using (var context = new ManagementSystemContext())
            {
                user = context.MstUsers.SingleOrDefault(u => u.UserName == userName);
                if (user != null)
                {
                    string encrypted = CryptoHelper.EncryptString(password, user.Salt);
                    if (string.Equals(user.UserPassword, password, StringComparison.Ordinal))
                    {
                        user.UserPassword = "";
                        user.Salt = "";
                    }
                    else {
                        user = null;
                    }
                }
            }
            return user;
        }

        public int CreateMstUser(MstUser user)
        {
            using (var context = new ManagementSystemContext())
            {
                user.UserId = Guid.NewGuid().ToString().Replace("-", "");
                string[] encrypted = CryptoHelper.EncryptString(char.ToUpper(user.UserName[0]) + user.UserName.Substring(1) + "**");
                user.Salt = encrypted[0];
                user.UserPassword = encrypted[1];

                var now = DateTime.Now;
                user.CreatedAt = now;
                user.ModifiedAt = now;

                context.MstUsers.Add(user);
                return context.SaveChanges();
            }
        }

        public int UpdateMstUser(MstUser user)
        {
            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstUsers.SingleOrDefault(u => u.UserName == user.UserName);
                if (existing == null) return 0;

                // update allowed fields
                existing.AccessList = user.AccessList;
                existing.IsBlacklist = user.IsBlacklist;
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = user.ModifiedBy;

                context.MstUsers.Update(existing);
                context.Entry(existing).Property(e => e.Salt).IsModified = false;
                context.Entry(existing).Property(e => e.UserPassword).IsModified = false;

                return context.SaveChanges();
            }
        }

        public bool ResetPassword(string userName, string modifiedBy)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstUsers.SingleOrDefault(u => u.UserName == userName);
                if (existing == null) return false;

                string[] encrypted = CryptoHelper.EncryptString(char.ToUpper(existing.UserName[0]) + existing.UserName.Substring(1) + "**");
                existing.Salt = encrypted[0];
                existing.UserPassword = encrypted[1];

                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = modifiedBy;

                context.MstUsers.Update(existing);
                return context.SaveChanges() > 0;
            }
        }

        public bool DeleteMstUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstUsers.SingleOrDefault(u => u.UserName == userName);
                if (existing == null) return false;

                context.MstUsers.Remove(existing);
                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Soft-delete by marking IsBlacklist = true and updating ModifiedAt/ModifiedBy.
        /// Returns true when save succeeded and user existed.
        /// </summary>
        public bool BlacklistUser(bool value, string userName, string modifiedBy)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstUsers.SingleOrDefault(u => u.UserName == userName);
                if (existing == null) return false;

                existing.IsBlacklist = value;
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = modifiedBy;

                context.MstUsers.Update(existing);
                return context.SaveChanges() > 0;
            }
        }
    }
}
