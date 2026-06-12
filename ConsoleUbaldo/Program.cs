using LoyaltyPoints.Data;
using LoyaltyPoints.Services;
using System;

namespace LoyaltyPoints.UI
{
    internal class Program
    {
        static LoyaltyService service = new LoyaltyService();

        static void Main(string[] args)
        {

            try
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("=====================================");
                    Console.WriteLine("       WELCOME TO NEIL'S SHOP        ");
                    Console.WriteLine("=====================================");
                    Console.Write("Are you a member? (Y/N): ");

                    string input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input))
                    {
                        continue;
                    }

                    char memberAns = Char.ToLower(input[0]);

                    if (memberAns == 'y')
                    {
                        if (Login()) LoyaltyMenu();
                    }
                    else if (memberAns == 'n')
                    {
                        CreateAccount();
                    }
                    else
                    {
                        Console.WriteLine("invalid input. Please enter Y or N.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[ERROR] Check if XAMPP MySQL is running!");
                Console.WriteLine("Details: " + ex.Message);
                Console.ResetColor();
                Console.WriteLine("\nPress Enter to exit...");
                Console.ReadLine();
            }
        }

        static void CreateAccount()
        {
            Console.Clear();
            Console.WriteLine("--- CREATE NEW ACCOUNT ---");
            Console.Write("Do you want to create an account? (Y/N): ");
            string input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Please enter Y or N.");
                Console.ReadLine();
                return;
            }

            char CreaAccAns = char.ToLower(input[0]);

            if (CreaAccAns == 'y')
            {
                Console.Write("Enter username: ");
                string username = Console.ReadLine();
                Console.Write("Enter password: ");
                string password = Console.ReadLine();

                service.CreateAccount(username, password);

                Console.WriteLine("\n[SUCCESS] Account saved to MySQL and JSON");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
            else if (CreaAccAns == 'n')
            {
                return;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter Y or N.");
                Console.ReadLine();
                return;
            }
        }

        static bool Login()
        {
            for (int i = 0; i < 3; i++)
            {
                Console.Clear();
                Console.WriteLine("--- MEMBER LOGIN ---");
                Console.Write("Username: ");
                string u = Console.ReadLine();
                Console.Write("Password: ");
                string p = Console.ReadLine();

                if (service.Login(u, p))
                {
                    Console.WriteLine("\nLogin Successful!");
                    System.Threading.Thread.Sleep(500);
                    return true;
                }

                else
                {
                    Console.WriteLine($"\nInvalid credentials. Attempts left: {2 - i}");
                    if (i < 2) Console.WriteLine("Press Enter to try again...");
                    Console.ReadLine();
                }
            }
            return false;
        }

        static void LoyaltyMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- LOYALTY POINTS MENU ---");
                Console.WriteLine("1. Add Points \n2. Use Points \n3. View Points \n4.Logout");
                Console.Write("\nChoice: ");

                string input = Console.ReadLine();
                int ans;

                if (!int.TryParse(input, out ans))
                {
                    Console.WriteLine("Invalid input.");
                    Console.ReadLine();
                }
                else if (ans == 1)
                {
                    AddPoints();
                }
                else if (ans == 2)
                {
                    UsePoints();
                }
                else if (ans == 3)
                {
                    ViewPoints();
                }
                else if (ans == 4)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                    Console.ReadLine();
                }
            }
        }

        static void AddPoints()
        {
            Console.Clear();
            Console.Write("\nEnter Total Spent: ");

            if (int.TryParse(Console.ReadLine(), out int spent))
            {
                int earned = service.AddPoints(spent);

                if (earned > 0)
                {
                    Console.WriteLine($"[SUCCESS] Earned {earned} points!");
                    Console.WriteLine($"[INFO] New Balance: {service.GetPoints()} points");
                }
                else
                {
                    Console.WriteLine("[INFO] Minimum spend is 500 to earn points.");
                }
            }
            else
            {
                Console.WriteLine("[ERROR] Invalid number.");
            }

            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        static void UsePoints()
        {
            Console.Clear();
            Console.WriteLine($"Current Balance: {service.GetPoints()} points");
            Console.WriteLine("=================================");
            Console.WriteLine("     SUPERMARKET MEMBER REWARDS");
            Console.WriteLine("=================================");
            Console.WriteLine("1. ₱20 Off Voucher (50 pts)");
            Console.WriteLine("2. ₱50 Off Voucher (120 pts)");
            Console.WriteLine("3. ₱100 Off Voucher (250 pts)");
            Console.WriteLine("4. 5% Discount Coupon (300 pts)");
            Console.WriteLine("5. Free Rice Pack (400 pts)");
            Console.WriteLine("6. Free Grocery Item (500 pts)");
            Console.WriteLine("7. Back to Menu");
            Console.WriteLine("=================================");
            Console.Write("Select Reward: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice == 7)
                {
                    return;
                }

                if (service.UsePoints(choice))
                {
                    Console.WriteLine("\n[SUCCESS] Reward redeemed successfully!");
                    Console.WriteLine($"[INFO] Remaining Points: {service.GetPoints()}");
                }
                else
                {
                    Console.WriteLine("\n[FAILED] Insufficient points or invalid choice.");
                }
            }
            else
            {
                Console.WriteLine("\n[ERROR] Invalid input.");
            }

            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }

        static void ViewPoints()
        {
            Console.Clear();
            Console.WriteLine("--- LOYALTY POINTS BALANCE ---");
            Console.WriteLine($"\nUser Account: Total Points = {service.GetPoints()}");
            Console.WriteLine("\nPress Enter to return...");
            Console.ReadLine();
        }
    }
}