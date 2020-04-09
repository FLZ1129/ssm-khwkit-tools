namespace CrazySharp.Base.Enums
{
    public enum UserRole
    {
        SUPER_ADMIN=1,
        FACTORY = 6,
        ADMIN = 11,
        ENGINEER = 16,
        DEFAULT = ENGINEER,
    }

    public static class UserRoleUtils
    {
        public static string GetUserRoleStr(UserRole userRole)
        {
            switch (userRole)
            {
                case UserRole.SUPER_ADMIN: return "超级管理员";
                case UserRole.FACTORY: return "工厂用户";
                case UserRole.ADMIN: return "管理员";
                case UserRole.ENGINEER: return "工程师";
            }
            return "未知";
        }
        public static UserRole GetUserRole(string userRoleStr)
        {
            switch (userRoleStr)
            {
                case "超级管理员": return UserRole.SUPER_ADMIN;
                case "工厂用户": return UserRole.FACTORY;
                case "管理员": return UserRole.ADMIN;
                case "工程师": return UserRole.ENGINEER;
            }
            return UserRole.DEFAULT;
        }
    }
}