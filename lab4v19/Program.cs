using System;

namespace Lab4
{
    // Базовий клас
    public class Account
    {
        private string _username;
        private string _passwordHash;

        public string Username
        {
            get => _username;
            set => _username = value;
        }

        public string PasswordHash
        {
            get => _passwordHash;
            set => _passwordHash = value;
        }

        public Account(string username, string passwordHash)
        {
            _username = username;
            _passwordHash = passwordHash;
        }

        public virtual void Login()
        {
            Console.WriteLine($"[Account]: Базовий вхід для користувача '{_username}'.");
        }

        public string GetAccountType()
        {
            return "Базовий акаунт (Base Account)";
        }
    }

    // Похідний клас 1
    public class UserAccount : Account
    {
        private string _email;

        public string Email
        {
            get => _email;
            set => _email = value;
        }

        public UserAccount(string username, string passwordHash, string email)
            : base(username, passwordHash)
        {
            _email = email;
        }

        public override void Login()
        {
            Console.WriteLine($"[UserAccount]: Вхід звичайного користувача '{Username}' (Email: {_email}).");
        }

        public void ChangePassword(string newPassword)
        {
            PasswordHash = "HASH_" + newPassword;
            Console.WriteLine($"[UserAccount]: Пароль для '{Username}' успішно змінено.");
        }

        // Демонстрація new: Приховування методу базового класу
        public new string GetAccountType()
        {
            return "Користувацький акаунт (User Account)";
        }
    }

    // Похідний клас 2
    public class AdminAccount : Account
    {
        private int _accessLevel;

        public int AccessLevel
        {
            get => _accessLevel;
            set => _accessLevel = value;
        }

        public AdminAccount(string username, string passwordHash, int accessLevel)
            : base(username, passwordHash)
        {
            _accessLevel = accessLevel;
        }

        public override void Login()
        {
            Console.WriteLine($"[AdminAccount]: Вхід АДМІНІСТРАТОРА '{Username}' з рівнем доступу {_accessLevel}.");
        }

        public void GrantPermissions(string user)
        {
            Console.WriteLine($"[AdminAccount]: Адмін '{Username}' надав розширені права користувачу '{user}'.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("1. СТВОРЕННЯ ОБ'ЄКТІВ ТА ВИКЛИК ВЛАСНИХ МЕТОДІВ");
            UserAccount user = new UserAccount("john_doe", "hash123", "john@example.com");
            AdminAccount admin = new AdminAccount("admin_boss", "adminhash", 10);

            user.ChangePassword("new_secret_pass");
            admin.GrantPermissions("john_doe");

            Console.WriteLine("\n2. ДЕМОНСТРАЦІЯ ПОЛІМОРФІЗМУ (override)");
            Account[] accounts = new Account[]
            {
                new Account("base_user", "basehash"),
                user,
                admin
            };

            foreach (Account acc in accounts)
            {
                // Завдяки override викликається відповідна реалізація для кожного типу
                acc.Login();
            }

            Console.WriteLine("\n3. ДЕМОНСТРАЦІЯ РІЗНИЦІ МІЖ OVERRIDE ТА NEW");
            
            // Виклик через посилання на похідний клас (UserAccount)
            Console.WriteLine("Виклик через посилання типу UserAccount:");
            Console.WriteLine($"GetAccountType(): {user.GetAccountType()}");

            // Виклик через посилання на базовий клас (Account)
            Account userAsBase = user;
            Console.WriteLine("Виклик того ж об'єкта через посилання типу Account:");
            Console.WriteLine($"GetAccountType(): {userAsBase.GetAccountType()}");
        }
    }
}
