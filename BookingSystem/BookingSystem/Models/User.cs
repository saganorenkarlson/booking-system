using System;
namespace BookingSystem.Models
{
	public class User
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }

		public User()
		{

		}

		public User(int id, string name, string email)
		{
			ID = id;
			Name = name;
			Email = email;
		}

		public override string ToString()
		{
			return ID + ", " + Name + ", " + Email;
		}
	}
}

