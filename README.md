# .NET-student-API

This API allows you to manage students.

Developed with:<br/>
👨‍💻  .NET CORE <br/>
💾  SQlite Database <br/>
⏳  Redis for distributed caching <br/>

Why Distributed Caching using Redis?
The server remembers HTTP requests for the last 10 sec.
if the same request comes up again, the response will be retrieved from Redis cache instead of querying the DB again, for better performance.

This is my first expirience with .NET CORE & Redis

## Endpoints ##

### Get all students ###

GET `/student`

Get all the students available.

### Get a single student ###

GET `/student/id`

Get all info about student.


### Submit a student ###

POST `/student`

Allows you to submit a new student. 
Student MUST be under 18.

The request body needs to be in JSON format and include the following properties:
```
{
  "id": 1,
  "firstName": "Or",
  "lastName": "Ganzian",
  "age": 17,
  "gradeAvarage": 98,
  "schoolName": "Brener",
  "schoolAdress": "Israel"
}
```

### Update a student ###

PUT `/student/`

Update an existing student.

The request body needs to be in JSON format and allows you to update the following properties:

 Example
```
{
  "id": 1,
  "firstName": "Or",
  "lastName": "Ganzian",
  "age": 17,
  "gradeAvarage": 98,
  "schoolName": "Brener",
  "schoolAdress": "Israel"
}
```

### Delete a student ###

DELETE `/student/id`

Delete an existing student. 

The request body needs to be in JSON format and allows you to update the following properties:

 Example
```
{
  "id": 1
}
```


