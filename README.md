# 📘 ICT2112-Software-Design-SD (ClearCare)
**ClearCare** is a web application designed to streamline the coordination and scheduling of pre-discharge services for patients. The system is structured into three main modules, each representing a distinct set of functionalities to support hospital discharge workflows.

## 🩺 About the Project
ClearCare simplifies the discharge planning process by offering role-based access for healthcare providers, centralized data management, service scheduling, and home safety assessments. It aims to improve communication between medical staff, patients, and caregivers—ensuring safer and more efficient patient transitions from hospital to home.

## 🧩 Modules Overview
### 🧾 Module 1: Account and Data Hub
- ⌨️ Register Patient / Caregiver accounts
- 🖥️ View Personal Medical Records
- 🙆 Patient/Caregiver Delegation
- 👨‍⚕️ Create Doctor/Nurses Accounts
- ⚙️ Manage Doctor/Nurses Account
- 🛠️ Audit Trail of Actions Performed by Any User
- 🔐 Secure Data Encryption + Database Handling
- 🖋️ File an erratum to update medical record
- 👤 View and Edit Profile
- 🙂 Login + Authentication
- 📁 Create Original Medical Record
- 📄 View and Export Medical Records

### 🏥 Module 2 - Care Service Orchestrator
- 📅: Manually Schedule Appointment
- 🤖: Automatically Schedule Appointment
- 👤: Manage Individual Schedule
- ⏳: Manage Appointment Backlog
- 🧭: Track Patient Pre-Discharge Services
- 📆: Manage Integrated Calendar
- 📊: Manage Dashboard Analytics
- 🔔: Manage Notifications
- 🎛️: Customize Notifications
- 🗃️: Manage Patient Service History
- ⚙️: Configure Service Types
- ✅: Manage Service Completion

### 💊 Module 3 - Med Track and Home Safe
- 💬 Medication counselling through reviewing of drug information
- 🏡 Home safety assessments for rehab team to assess risks and recommend modifications
- ☑️ Virtual checklist provided for asessment
- 📄 Documentation of discussions and recommendations for the assessment
- 🗣️ Feedback system
- 🌐 Community hub for patients and caregivers
- ❓ Enquiry system
- 😷 Side Effect Logging & Analytics
- 📝 Drug Information Management
- 🧪 Uploading detailed drug information 
- ⚠️ Drug interaction checker 
- 📋 View and document patient discharge information and summaries  
- 🩺 Manage consultation sessions between doctors and patients  
- 📚 Provide accessible educational materials for patients and caregivers

### 📁 Project Structure
```bash
├───Controllers     # Controllers
├───DataSource      # DataSource
├───Models          # Models
├───Views           # Views (UI) of the project
│   └───Shared      # Base layout, partials
└───wwwroot
    ├───css         # CSS files
    ├───js          # JS files
```

### 🛠 Built With
- ASP.NET Core  
- C#  
- Razor Pages  
- Entity Framework Core  
- Firebase

## 🚀 Getting Started

### ✅ Prerequisites
1. Dotnet SDK

### 🧪 Setup Instruction

#### 🔥 Firebase Setup
1. Log into firebase console (https://console.firebase.google.com/u/0/)
2. Click **Project Settings** 
3. Click **Service Accounts** >**Generate new private key**
4. **Rename** .json file to "**firebase-adminsdk.json**"
5. Insert **firebase-adminsdk.json** to "**/ClearCare/Firebase**" folder

❗️**Do not** commit firebase-adminsdk to github repository.

#### 🖥️ Application Setup
1. Clone the repository:
```bash
   git clone https://github.com/jyboat/ICT2112-Software-Design-SD
   cd ClearCare
```

2. Checkout the module you want to run:
   - 🧾 Module 1 (Account and Data Hub)
     ```bash
     git checkout Module-1
     ```
   - 🏥 Module 2 (Care Service Orchestrator)
     ```bash
     git checkout Module-2
     ```
   - 💊 Module 3 (Med Track and Home Safe)
     ```bash
     git checkout Module-3
     ```

3. Run the application:
```bash
    dotnet run
```

4. Open your browser and visit:
```bash
    http://localhost:5007
```

