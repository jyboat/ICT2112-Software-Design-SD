# ClearCare Project - README

## Table of Contents
1.  [Overview](#overview)
2.  [Features](#features)
3.  [Technologies Used](#technologies-used)
4.  [Setup Instructions](#setup-instructions)
    *   [Prerequisites](#prerequisites)
    *   [Configuration](#configuration)
    *   [Install Dependencies](#install-dependencies)
    *   [Build the Project](#build-the-project)
    *   [Run the Project](#run-the-project)
5.  [Code Structure](#code-structure)
6.  [Key Components](#key-components)
    *   [Controllers](#controllers)
    *   [Gateways (Mappers)](#gateways-mappers)
    *   [Controls](#controls)
7.  [Observer Pattern](#observer-pattern)
8.  [User Switching](#user-switching)
9.  [External API Integration](#external-api-integration)
10. [API Documentation](#api-documentation-external-api-drug-interaction-and-side-effect-service-portfolio-website-lyart-five-75vercelapp)
11. [Notes](#notes)


## Overview

ClearCare is a web application designed to streamline the coordination and scheduling of pre-discharge services for patients. The system is structured into three main modules, each representing a distinct set of functionalities to support hospital discharge workflows.

This module (Med Track and Home Safe) focuses on improving communication about medications and home safety, including enquiries, prescriptions, side effects, and drug interactions. It integrates with Firebase Firestore for data storage and utilizes external APIs for drug interaction, zoom consultations, and side effect information.

## Features

### Team 1

*   **Home safety assessment system**
    *   Enables submission of living conditions for assessment
    *   Doctors in rehab team may assess living conditions without face-to-face visits
    *   Provide recommendations and go through a checklist

*   **Virtual consultation sessions**
    *   Integration of Zoom for online consultations for appointments and follow-ups

*   **Discharge summaries**
    *   Summarizes a patient's discharge process and procedures, along with any prescriptions

*   **Resource library**
    *   Provide educational health-related resources for patients or caregivers to view
    *   Allow healthcare professionals to create or edit resources

*   **Community hub**
    *   Foster discussions and peer support among patients or caregivers
    *   Allows creation of community groups, with posts and comments

*   **Feedback system**
    *   Patients or caregivers can send feedback on the discharge process
    *   Doctors and Nurses can reply
    *   Notifies patient or caregiver on response

### Team 2

*   **Enquiry Management:**
    *   Patients can submit enquiries to doctors.
    *   Doctors can view and respond to patient enquiries.
    *   Enquiries are stored in Firestore.
    *   Supports pagination for long lists of replies.
*   **Prescription Management:**
    *   Doctors can create and manage prescriptions.
    *   Prescriptions are stored in Firestore.
    *   Users (Doctors and Patients) can only view their own Prescriptions
*   **Side Effect Tracking:**
    *   Patients can report side effects for medications.
    *   Side effects are stored in Firestore.
    *   Provides charting capabilities to visualize side effect data.
*   **Drug Interaction Checking:**
    *   Checks for potential drug interactions using an external API.
    *   Allows uploading new drug interactions.
*   **Patient Drug Log:**
    *   Patients and Doctors can add/view to drug log.
*   **User Authentication and Roles:**
    *   Supports different user roles (Doctor and Patient).
    *   Uses session management to maintain user state.
    *   User switching functionality for demonstration purposes.

## Technologies Used

*   **ASP.NET Core MVC:**  For building the web application.
*   **Firebase Firestore:**  For data storage.
*   **Firebase Admin SDK:**  For interacting with Firestore.
*   **HttpClient:**  For making HTTP requests to external APIs.
*   **Observer Pattern:** For decoupling components.
*   **Prettier:** Used to format the code.

## Setup Instructions

1.  **Prerequisites:**
    *   .NET SDK (version 7.0 or later).
    *   Firebase project with Firestore enabled.
    *   A service account key file for Firebase.

2.  **Configuration:**
    *   Clone the repository.
    *   Add the Firebase service account key file to the project.
    *   Configure the Firebase project ID in the code.
        *   Check the \*.cs files where `FirestoreDb.Create("ict2112")` is used, and replace `ict2112` with your Firebase project ID.
    *   Configure the session.

3.  **Install Dependencies:**

    ```bash
    dotnet restore
    ```

4.  **Build the Project:**

    ```bash
    dotnet build
    ```

5.  **Run the Project:**

**Steps**
1. Clone or download the project from GitHub
2. Run the following:
```bash
cd ClearCare    # Change directory to project
dotnet run      # Run the applciation
```
3. View the application on localhost

#### Firebase Setup
1. Log into firebase console (https://console.firebase.google.com/u/0/)
2. Click **Project Settings** 

![alt text](Readme_Images/Firebase_Project_Settings.png)

3. Click **Service Accounts** >**Generate new private key**

![alt text](Readme_Images/Firebase_ServiceAccount_Tab.png)

4. **Rename** .json file to "**firebase-adminsdk.json**"

5. Insert **firebase-adminsdk.json** to "**/ClearCare/Firebase**" folder


![alt text](Readme_Images/Firebase_Secret.png)

<span style="color: red;">**Do not**</span> commit firebase-adminsdk to github repository

