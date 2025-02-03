# ICT2112-Software-Design-SD

## ClearCare

ClearCare is a Web application designed to stramline the coordination and scheduling of pre-discharge services for patients.
The application is divided into 3 modules:

### Module 1 - Account and Data Hub

- Handling of accounts, profiles, role-based access
- Personalized profile page to view and edit information
- Admins are able to reset passwords, delete accounts, and perform other adminnistrative tasks
- Handling of patient medical records
- Filing of erratum upon amendment of records
- Secure storage and communication of data

### Module 2 - Care Servie Orchestrator

- Assignment of nurses to pre-discharge services
- Automated reminders and notifications for patients on upcoming service appointments
- Recording of service history for patients

### Module 3 - Med Track and Home Safe

- Medication counselling through reviewing of drug information
- Home safety assessments for rehab team to assess risks and recommend modifications
- Virtual checklist provided for asessment
- Documentation of discussions and recommendations for the assessment
- Online zoom integrated consultations


### Additional Features
- Feedback system
- Enquiry system
- Community hub for patients and caregivers
- Discharge summary generator

### Project Structure
```bash
├───Controllers     # Controllers
├───Models          # Models
├───Views           # Views (UI) of the project
│   └───Shared      # Base layout, partials
└───wwwroot
    ├───css         # CSS files
    ├───js          # JS files
```

### Project Setup
**Prerequisites**
1. Dotnet SDK

**Steps**
1. Clone or download the project from GitHub
2. Run the following:
```bash
cd ClearCare    # Change directory to project
dotnet run      # Run the applciation
```
3. View the application on localhost