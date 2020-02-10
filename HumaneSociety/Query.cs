﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            crudOperation = crudOperation.ToLower();
            //Need switch statement for all four CRUD operations
            switch(crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    break;
                case "remove":
                    List<Employee> empRemove = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).ToList();
                    db.Employees.DeleteOnSubmit(empRemove[0]);
                    //Again, probably wrong, but works
                    break;
                case "update":
                    List<Employee> empUpdate = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).ToList();
                    db.Employees.InsertOnSubmit(empUpdate[0]);
                    //Without question, not the right way to do this. But it seems to work.
                    break;
                case "display":
                    db.Employees.Select(x => x.EmployeeNumber == employee.EmployeeNumber);
                    break;
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static Animal GetAnimalByID(int id)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {            
            throw new NotImplementedException();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static List<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            //Proabably not allowed to do this. Changed the return type here from IQueryable<Animal> to List<Animal> because, in all three places it's used, it's converted immediately to a list.

            var matchingAnimals = db.Animals.ToList();

            foreach (KeyValuePair<int, string> pair in updates)
            {
                switch (pair.Key)
                {
                    case 1:
                        matchingAnimals.RemoveAll(a => a.Category.ToString() != pair.Value);
                        //Want it to be not equal. Probably need to do some kind of conversion or something.
                        //Doesn't quite work because it's testing each one against all of the values. I only want the values at 1, which is what they chose in this case
                        break;
                    case 2:
                        matchingAnimals.RemoveAll(a => a.Name.ToString() != pair.Value);
                        //Don't want where. I want a function to remove the items I don't want, that way I can use the same variable each time, and it will narrow as it moves down the list
                        break;
                    case 3:
                        matchingAnimals.RemoveAll(a => a.Age.ToString() != pair.Value);
                        break;
                    case 4:
                        matchingAnimals.RemoveAll(a => a.Demeanor.ToString() != pair.Value);
                        break;
                    case 5:
                        matchingAnimals.RemoveAll(a => a.KidFriendly.ToString() != pair.Value);
                        break;
                    case 6:
                        matchingAnimals.RemoveAll(a => a.PetFriendly.ToString() != pair.Value);
                        break;
                    case 7:
                        matchingAnimals.RemoveAll(a => a.Weight.ToString() != pair.Value);
                        break;
                    case 8:
                        matchingAnimals.RemoveAll(a => a.AnimalId.ToString() != pair.Value);
                        break;
                }
            }
            return matchingAnimals;
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            //Need to take in the categoryName and return the categoryId. Test the name against the category, and return the categoryId
            string stringId = db.Animals.Where(a => a.Category.Equals(categoryName)).ToString();
            int categoryId = int.Parse(stringId);
            return categoryId;
        }
        
        internal static Room GetRoom(int animalId)
        {
            //Returns Room object when given an animal's id number
            var animalRoom = db.Rooms.Where(r => r.AnimalId.Equals(animalId)).FirstOrDefault();
            return animalRoom;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            var planID = db.DietPlans.Where(d => d.Name.Equals(dietPlanName)).FirstOrDefault();
            return planID.DietPlanId;
            //I don't think this is right

        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}