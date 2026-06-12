using LoyaltyPoints.Data;
using LoyaltyPoints.Models;

namespace LoyaltyPoints.Services
{
    public class LoyaltyService
    {
        //database acces
        private readonly UserRepository _repo = new UserRepository();

        public User CurrentUser { get; private set; }

        public LoyaltyService() { }

        //login
        public bool Login(string username, string password)
        {
            CurrentUser = _repo.GetUser(username, password);
            return CurrentUser != null;
        }

        //create account
        public void CreateAccount(string username, string password)
        {
            _repo.CreateUser(username, password);
        }

        //get points
        public int GetPoints()
        {
            return CurrentUser?.Points ?? 0;
        }

        //add points
        public int AddPoints(int moneySpent)
        {
            if (CurrentUser == null) return 0;
            if (moneySpent < 500) return 0;

            int earned = moneySpent / 100;

            if (moneySpent >= 5000)
            {
                earned += 20;
            }

            if (moneySpent >= 10000)
            {
                earned += 50;
            }

            CurrentUser.Points += earned;
            _repo.UpdatePoints(CurrentUser.Id, CurrentUser.Points);

            return earned;
        }

        //use points
        public bool UsePoints(int option)
        {
            if (CurrentUser == null)
            {
                return false;
            }

            int cost = 0;

            if (option == 1)
            {
                cost = 50;
            }
            else if (option == 2)
            {
                cost = 120;
            }
            else if (option == 3)
            {
                cost = 250;
            }
            else if (option == 4)
            {
                cost = 300;
            }
            else if (option == 5)
            {
                cost = 400;
            }
            else if (option == 6)
            {
                cost = 500;
            }
            else
            {
                return false;
            }

            if (CurrentUser.Points >= cost)
            {
                CurrentUser.Points -= cost;
                _repo.UpdatePoints(CurrentUser.Id, CurrentUser.Points);
                return true;
            }

            return false;
        }
        public bool DeleteAccount(int id)
        {
            var repo = new LoyaltyPoints.Data.UserRepository();
            return repo.DeleteUser(id);
        }
    }
}