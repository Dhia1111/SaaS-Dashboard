using Connection.models;
using Microsoft.Extensions.Logging;




namespace Business
    {
        public interface IPersonService
        {
            Task<IReadOnlyList<DtoPerson>> GetAllAsync();

            Task<DtoPerson> GetByIdAsync(int id);

            Task<DtoPerson> GetByEmailAsync(string Email);


        Task<IReadOnlyList<DtoPerson>> SearchByNameAsync(string name);

            Task AddAsync(Person person);

            Task UpdateAsync(Person person);

            Task DeleteAsync(int id);
    }
    

        public class clsPersonService : IPersonService
        {
            private readonly IPersonRepository _repository;
            private readonly ILogger _logger;

            public clsPersonService(IPersonRepository repository,ILogger logger)
            {
                _repository = repository;
                _logger = logger;
            }

            public async Task<IReadOnlyList<DtoPerson>> GetAllAsync()
            {
                var people = await _repository.GetAllAsync();
                return people.Select(ToDto).ToList();
            }

            public async Task<DtoPerson> GetByIdAsync(int id)
            {
                var person = await _repository.GetByIdAsync(id);

                if (person == null)
                    throw new KeyNotFoundException($"Person with id {id} not found.");

                return ToDto(person);
            }
        public async Task<DtoPerson> GetByEmailAsync(string Email)
        {
            var person = await _repository.GetByEmailAsync(Email);

            if (person == null) {
               
                throw new KeyNotFoundException($"Person with id {Email} not found.");
                }
            return ToDto(person);
        }
        public async Task<IReadOnlyList<DtoPerson>> SearchByNameAsync(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Search name is required.");

                var people = await _repository.SearchByNameAsync(name.Trim());
                return people.Select(ToDto).ToList();
            }

            public async Task AddAsync(Person person)
            {
            _logger.LogInformation("Creating person with Email {Email}", person.Email);

            var existing = await _repository.GetByEmailAsync(person.Email);

            if (existing != null)
            {
                _logger.LogWarning(
                "Attempt to create duplicate person with Email {Email}",
                person.Email);
                throw new InvalidOperationException("Email already exists.");
            }

                await _repository.AddAsync(person);
            _logger.LogInformation(
         "Person created successfully with Id {PersonId}",
         person.Id);
        }

            public async Task UpdateAsync(Person person)
            {
                var existing = await _repository.GetByIdAsync(person.Id);
                if (existing == null)
                    throw new KeyNotFoundException("Person not found.");

                await _repository.UpdateAsync(person);
            }

            public async Task DeleteAsync(int id)
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                    throw new KeyNotFoundException("Person not found.");

                await _repository.DeleteAsync(existing);
            }

            private  DtoPerson ToDto(Person person)
            {
                return new DtoPerson
                {
                    Id = person.Id,
                    Email = person.Email,
                    Phone = person.Phone,
                    FirstName = person.FirstName,
                    LastName = person.LastName
                };
            }
        }
    }






