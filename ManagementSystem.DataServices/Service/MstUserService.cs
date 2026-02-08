
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

            if (string.IsNullOrWhiteSpace(model.UserId))
                validations.Add(ModelValidation.FieldIsRequired(nameof(MstUser), nameof(model.UserId), model.UserId));

            // Validasi taro dibawah karena Delete cuma terisi PK nya saja
            if (action != ServiceAction.Delete)
            {
                if (string.IsNullOrWhiteSpace(model.UserName))
                    validations.Add(ModelValidation.FieldIsRequired(nameof(MstUser), nameof(model.UserName), model.UserName));

                if (IsExists(action, model))
                    validations.Add(ModelValidation.PKAlreadyExists(nameof(MstUser), nameof(model.UserId), model.UserId));

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
                    return context.MstUsers.Any(u => u.UserId == model.UserId);
                }

            return false;
        }

        public MstUser? GetMstUserById(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            using (var context = new ManagementSystemContext())
            {
                MstUser? user = context.MstUsers.SingleOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    user.UserPassword = "";
                    user.Salt = "";
                }
                return user;
            }
        }

        public int CreateMstUser(MstUser user)
        {
            using (var context = new ManagementSystemContext())
            {
                string[] encrypted = CryptoHelper.EncryptString(char.ToUpper(user.UserId[0]) + user.UserId.Substring(1) + "**");
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
                var existing = context.MstUsers.SingleOrDefault(u => u.UserId == user.UserId);
                if (existing == null) return 0;

                // update allowed fields
                existing.UserName = user.UserName;
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

        public bool ResetPassword(string userId, string modifiedBy)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstUsers.SingleOrDefault(u => u.UserId == userId);
                if (existing == null) return false;

                string[] encrypted = CryptoHelper.EncryptString(char.ToUpper(existing.UserId[0]) + existing.UserId.Substring(1) + "**");
                existing.Salt = encrypted[0];
                existing.UserPassword = encrypted[1];

                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = modifiedBy;

                context.MstUsers.Update(existing);
                return context.SaveChanges() > 0;
            }
        }

        public bool DeleteMstUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstUsers.SingleOrDefault(u => u.UserId == userId);
                if (existing == null) return false;

                context.MstUsers.Remove(existing);
                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Soft-delete by marking IsBlacklist = true and updating ModifiedAt/ModifiedBy.
        /// Returns true when save succeeded and user existed.
        /// </summary>
        public bool BlacklistUser(bool value, string userId, string modifiedBy)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstUsers.SingleOrDefault(u => u.UserId == userId);
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
