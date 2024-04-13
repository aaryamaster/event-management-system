# Eventify - Event Management System

## Abstract

Eventify is a comprehensive Event Management System developed using ASP.NET MVC Core, Entity Framework, and SQL Server. It provides users with a versatile platform to create, manage, and participate in various events. With a focus on user engagement and administrative efficiency, Eventify offers a seamless experience for event organizers and participants alike.

## Key Features

1. **User Registration and Authentication**: Secure account creation and authentication mechanisms ensure a safe and protected user experience.
   
2. **Private Event Creation**: Registered users can organize their private events, customizing details such as event name, date, venue, and additional services.
   
3. **Admin Event Creation**: Administrators have the authority to create and manage admin events, facilitating ticket purchases and event coordination.
   
4. **Dashboard Functionality**: Both users and administrators benefit from personalized dashboards, providing insights into registered events, ticket purchases, and event management tools.
   
5. **Event Participation**: Users can explore and participate in a diverse range of upcoming events, spanning various categories and interests.
   
Eventify aims to streamline event management processes, enhancing user engagement and administrative effectiveness.

## Project Goals

- Establish a user-friendly online platform for event enthusiasts to create and participate in events seamlessly.
  
- Digitize and optimize event management operations, improving the overall experience for organizers and participants.
  
- Ensure data security and confidentiality throughout the system's operation.

## Entity Relationship

1. **One-to-Many Relationship**:
   - Between Events and Halls: Each event can be associated with one hall, facilitating venue management and event organization.

2. **Many-to-Many Relationship**:
   - Between Users and Roles: Users can have multiple roles, enabling flexible access control and permission management.

3. **One-to-Many Relationship (Bidirectional)**:
   - Between Events and Tickets: Events can have multiple tickets, allowing for efficient ticket management and sales tracking.

4. **Many-to-Many Relationship (Bidirectional)**:
   - Between Users and Events: Users can participate in multiple events, fostering community engagement and interaction.

## Screenshots

### Home Page
![Home Page](![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/93335399-b728-46b7-ae89-3fab8575ead0))


### Browse Event Page
![Browse Event Page](![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/c1c5ecc5-f986-4c90-af6e-7203fbd3f683)
)

### Private Event Creation Page
![Private Event Creation Page](![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/207670bb-04c2-4286-9d54-273847bd312e)
)

### User Dashboard
![User Dashboard](![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/067f3b35-ba9a-4f29-9a1b-88231e544226)
)

### User Organized Events
![User Organized Events](![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/c9b5e0c7-fca0-411f-8be7-de990ead7d85)
)

### User Purchased Events
![User Purchased Events](![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/a20536a2-dcef-4511-8c50-fe35d7a924c4)
)

### Admin Dashboard
![Admin Dashboard](![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/cf4583ee-35f2-44de-8f63-c310566cb59f)
)

### User Events
![User Events](![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/2d596a62-e123-40bd-bccd-b0de2bd75cf6)
)

### Admin Events
![Admin Events](![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/47c3c872-ecf6-4799-a5c2-a084f98d845a)
)

## Database Schema and Dataflow Diagram

- **Database Schema**: [![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/49f3533d-e6a1-47b8-a703-4fb1c29b73f9)
]
- **Dataflow Diagram**: [![image](https://github.com/Shrey5555/EventManagementSystemProject/assets/136813149/129a376e-4570-48ac-b987-d26c81428a21)
]
