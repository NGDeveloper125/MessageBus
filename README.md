# MessageBus
This project aims to develop a generic message bus for multiple microservice/services infrastructure, guided by the principles of Test-Driven Development (TDD) and Domain-Driven Design (DDD). 
The message bus facilitates efficient and reliable communication between services in a clean and maintainable manner.

### Technologies Used
- Programming Language: C#
- .NET Version: 8
- Socket Library: NetMQ
- Testing Framework: xUnit & NSubstitute & FluentAssertions
- Logging Library: Serilog

## Project Overview
The MessageBus project focuses on creating a robust and scalable message bus that enables seamless communication between multiple services. By leveraging functional programming techniques, the codebase is designed to be highly modular, testable, and maintainable. The adoption of TDD ensures that the system is thoroughly tested and reliable, while DDD principles help in creating a domain-centric clean architecture that aligns with the business requirements.

### Projects included -
- **Domain:** librery project that holds the bus logic including all records, entities, extensions and the messagebus object.
- **MessageBusHost:** a console project that runs the messagebus as a windows service without any interface.
- **MessageBusTests:** a xunit tests project that include unit tests for the domain and integration tests for the messagebusfacade.

### Project Featurs -
- **Generic Communication:** The message bus provides a generic interface for communication, allowing services to exchange messages without tight coupling.
- **Id/Topic Base Messages:** The messages can be pushed and pull by topic, in a normal services structure, or by id for processes that need to have a more specific approach.
- **Queue Recovery:** When the message bus is going down, all messages on the queue will be saved in a local file and will be return to queue when message bus is back on. 

### Project Components -
- **MessageBusHost:** a class that derived of the BackgroundService base class, host the messagebus, embuser and debuser. 
- **MessageBus:** a class that holds the message queue and handle embusing and debusing messages.
- **Embuser:** a listening object for push messages.
- **Debuser:** a listening object for pull request messages.
- **MessageBusFacade:** a client side object to push and pull messages from and to the bus (this object should be implemented at any service that wish to use the bus)
  
### Implementation Guid Lines - 
- **Domain-Driven Design:** The message bus is designed with DDD principles in mind, focusing on the core domain concepts and their interactions.
- **Functional Programming:** The codebase is implemented using functional programming principles, emphasizing immutability, pure functions, and composability.
- **Test-Driven Development:** The development process follows a TDD approach, ensuring comprehensive test coverage and enabling confident refactoring. 

### Future Featurs -
- **Interface:** either a console or GUI for running the messagebus in the front with continuous state update of queue and sockets.

### Contributing:
Contributions to the MessageBus project are welcome! If you encounter any issues, have suggestions for improvements, or would like to add new features, please submit a pull request. Ensure that your contributions align with the project's coding standards and include appropriate tests.

### Contact:
For any inquiries or questions regarding the MessageBus project, please contact me (Nimrod) at NGDeveloper@OutLook.com
