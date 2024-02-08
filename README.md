## Multiple Choice Exam App

This project implements an exam system consisting of a server and a client application. The system allows administrators to manage exam questions and conduct exams for students.

## Features

- **Server Application**: Allows administrators to add, edit, and view exam questions, as well as conduct exams for students.
- **Client Application**: Provides students with an interface to take exams, view exam results, and receive notifications.

## Installation

1. Clone the repository: `git clone https://github.com/viksa27/Multiple-Choice-Test-App.git`
2. Open the solution in Visual Studio (ExamProjectCSharp.sln) and build all 3 projects.
4. Run the server application executable (`ExamServerWPF.exe`).
5. Run the client application executable (`ExamClientWPF.exe`).

## Usage

### Server Application

1. Launch the server application.
2. Add or import exam questions.   
*You can use Multiple-Choice-Exam-App/ExamServerWPF/bin/Debug/net7.0-windows/QuestionBank.xml as an example for importing format*
3. Start an exam session.
4. Monitor exam sessions and view results.

### Client Application

1. Launch the client application.
2. Log in with student credentials.
3. Take assigned exams.
4. View exam results.

*Note: the server must be already running before starting the exam*

## Configuration

- **Server IP Address**: Configure the IP address of the server in the client application settings.  
*Note: if the server and client are on different computers, windows firewall should be configured to allow the connection*
