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
10. [Areas for Improvement](#areas-for-improvement)
11. [Notes](#notes)

## Overview

ClearCare is an ASP.NET Core MVC application designed to manage various aspects of patient care, including enquiries, prescriptions, side effects, and drug interactions. It integrates with Firebase Firestore for data storage and utilizes external APIs for drug interaction and side effect information.

## Features

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

    ```bash
    dotnet run
    ```

    The application will be accessible at `http://localhost:5007` (or another port specified in your launch settings).

## Code Structure

The project follows a standard ASP.NET Core MVC structure, with the following key directories:

*   **Controllers:** Contains the controller classes for handling user requests and interactions.
*   **Models:** Contains the data models representing the application's entities (e.g., `Enquiry`, `PrescriptionModel`, `SideEffectModel`).
*   **Views:** Contains the Razor views for rendering the user interface.
*   **Gateways:** Contains classes for interacting with the data store (Firestore).  These act as Mappers.
*   **Interfaces:** Contains interfaces defining contracts for different services and functionalities.
*   **Controls:** Contains the business logic and control classes for managing the application's features.
*   **Observer:** Contains interfaces and implementations related to the observer pattern.

## Key Components

*   **`EnquiryController`:** Handles the submission, retrieval, and management of enquiries.
*   **`PrescriptionController`:** Handles the creation, retrieval, and display of prescriptions.
*   **`SideEffectsController`:** Handles the reporting, retrieval, and visualization of side effects.
*   **`DrugInteractionController`:** Handles the checking and uploading of drug interactions.
*   **`PatientDrugLogController`:**  Handles the Patient drug log
*   **`ApplicationController`:**  Acts as main entrypoint of the application.
*   **`UserSwitcherController`:** Used for easily switching between user accounts for demonstration purposes.
*   **`HomeController`:**  Handles the default index and error pages.

*   **`EnquiryGateway`:** Interacts with Firestore to store and retrieve enquiry data.
*   **`PrescriptionMapper`:** Interacts with Firestore to store and retrieve prescription data.
*   **`SideEffectsMapper`:** Interacts with Firestore to store and retrieve side effect data.
*   **`PatientDrugMapper`:**  Interacts with Firestore to store and retrieve patient drug log data.
*   **`DrugLogSideEffectsService`:**  Fetches drug side effects from the external API.

*   **`EnquiryControl`:** Manages the business logic for enquiries.
*   **`PrescriptionControl`:** Manages the business logic for prescriptions.
*   **`SideEffectControl`:** Manages the business logic for side effects.
*   **`DrugInteractionControl`:** Manages the business logic for drug interactions.
*   **`PatientDrugLogControl`:**  Manages the patient drug log business logic.

## Observer Pattern

The project implements the observer pattern to decouple components and facilitate event-driven behavior. The `IObserver<T>` and `ISubject<T>` interfaces are used to define the observer and subject roles, respectively.  The `EnquiryControl` and `SideEffectControl` make use of the Observer pattern.

## User Switching

The `UserSwitcherController` and `UserSwitcherService` components are provided for demonstration purposes, allowing you to easily switch between different user accounts (Doctor and Patient) without needing full authentication.  **This is purely for demonstration and should not be used in a production environment.**

## External API Integration

The `DrugInteractionControl` and `DrugLogSideEffectsService` components integrate with external APIs to retrieve drug interaction and side effect information.  **Ensure that these APIs are available and properly configured.**

## Areas for Improvement

*   Implement full user authentication and authorization.
*   Implement proper error handling and logging.
*   Add unit tests to improve code quality and reliability.
*   Implement input validation and sanitization to prevent security vulnerabilities.
*   Securely manage API keys and connection strings.
*   Improve the user interface and user experience.
*   Implement robust data validation.
*   Implement data pagination in all features.

## Notes

*   This project provides a basic framework for managing patient care data.
*   It can be extended and customized to meet specific requirements.
*   It is important to address the security considerations and areas for improvement before deploying the application in a production environment.
*   **The `portfolio-website-lyart-five-75.vercel.app` API, where the project pings to get setup, is a self-made API specifically for this project's purpose.  Ensure it is running and configured correctly for the Drug Interaction and Side Effect features to work.**
