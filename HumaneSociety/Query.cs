using System;
using System.IO;
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
            switch(crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                case "delete":
                    Employee empRemove = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    db.Employees.DeleteOnSubmit(empRemove);
                    //This just tries to remove the employee. Need to remove the entire row and save the changes
                    db.SubmitChanges();
                    break;
                case "update":
                    Employee empUpdate = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    db.Employees.InsertOnSubmit(empUpdate);
                    db.SubmitChanges();
                    break;
                case "read":
                    Employee tempEmployee = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    Console.WriteLine($"First Name: {tempEmployee.FirstName}, Last Name: {tempEmployee.LastName}, Employee ID: {tempEmployee.EmployeeId}, Employee Email: {tempEmployee.Email}");
                    Console.ReadLine();
                    break;
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
            //Currently doesn't get gender, adoptionstatus, and employeeid, as required on the table. Is there a way to add them here?
        }

        internal static Animal GetAnimalByID(int id)
        {
            var retrieveAnimal = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();                 
            return retrieveAnimal;   
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            List<Animal> animalUpdate = db.Animals.Where(a => a.AnimalId == animalId).ToList();
            db.Animals.DeleteOnSubmit(animalUpdate[0]);
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            List<Animal> animalRemove = db.Animals.Where(e => e.AnimalId == animal.AnimalId).ToList();
            db.Animals.DeleteOnSubmit(animalRemove[0]);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IEnumerable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates)
        {
            var matchingAnimals = db.Animals.ToList();

            foreach (KeyValuePair<int, string> pair in updates)
            {
                switch (pair.Key)
                {
                    case 1:
                        matchingAnimals.RemoveAll(a => a.Category.Name != pair.Value && a.Category.Name != pair.Value.ToLower());
                        break;
                    case 2:
                        matchingAnimals.RemoveAll(a => a.Name != pair.Value && a.Name != pair.Value.ToLower());
                        break;
                    case 3:
                        matchingAnimals.RemoveAll(a => a.Age.ToString() != pair.Value && a.Age.ToString() != pair.Value.ToLower());
                        break;
                    case 4:
                        matchingAnimals.RemoveAll(a => a.Demeanor != pair.Value && a.Demeanor != pair.Value.ToLower());
                        break;
                    case 5:
                        matchingAnimals.RemoveAll(a => a.KidFriendly.ToString() != pair.Value && a.KidFriendly.ToString() != pair.Value.ToLower());
                        break;
                    case 6:
                        matchingAnimals.RemoveAll(a => a.PetFriendly.ToString() != pair.Value && a.PetFriendly.ToString() != pair.Value.ToLower());
                        break;
                    case 7:
                        matchingAnimals.RemoveAll(a => a.Weight.ToString() != pair.Value && a.Weight.ToString() != pair.Value.ToLower());
                        break;
                    case 8:
                        matchingAnimals.RemoveAll(a => a.AnimalId.ToString() != pair.Value && a.AnimalId.ToString() != pair.Value.ToLower());
                        break;
                }
            }
            return matchingAnimals;
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            var animal = db.Animals.Where(a => a.Category.Name.Equals(categoryName)).FirstOrDefault();
            return animal.CategoryId.GetValueOrDefault();
            //Rewrite if time
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
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption();
            adoption.ClientId = client.ClientId;
            adoption.AnimalId = animal.AnimalId;
            adoption.ApprovalStatus = "Not Adopted";
            adoption.AdoptionFee = 100;
            adoption.PaymentCollected = false;
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            var pendingAdoptions = db.Adoptions.Select(a => a);
            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if (isAdopted)
            {
                var updateAdoptedAnimalAdoptionStatus = db.Adoptions.Select(a => a.AnimalId).FirstOrDefault();
                foreach (Animal animal in db.Animals)
                {
                    if (updateAdoptedAnimalAdoptionStatus == animal.AnimalId)
                    {
                        animal.AdoptionStatus = "Is adopted";
                    }
                }
                //A better way to do this? Some kind of LINQ statement?
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption adoption = new Adoption();
            var adoptionToDelete = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).FirstOrDefault();
            db.Adoptions.DeleteOnSubmit(adoptionToDelete);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var animalShots = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId);
            return animalShots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot shot = new AnimalShot();
            shot.AnimalId = animal.AnimalId;
            shot.ShotId = db.Shots.Where(s => s.Name == shotName).FirstOrDefault().ShotId;
            shot.DateReceived = DateTime.Now;

            animal.AnimalShots.Add(shot);
            db.SubmitChanges();
            //Is this necessary after .Add()?
        }

        internal static void AddAnimalsFromCSVFile()//Needs to take in the CSV file here probably
        {
            CSVOpener opener = new CSVOpener();
            List<Animal> animalList = opener.LoadCSV();
            foreach (Animal animal in animalList)
            {
                db.Animals.InsertOnSubmit(animal);
                db.SubmitChanges();
            }
            //Might want to take in a csv file or something in the constructor??
        }
    }
}