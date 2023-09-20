# Bank API Documentation

## Authentication

To access protected endpoints, you need to include an authentication token in your request headers. Use the `Bearer` token format followed by the actual token:

Authorization: Bearer <your_token_here>


## Admin Account

You must create an admin account manually. To do this, follow these steps:

1. Register a customer account by making a POST request to `/api/Customer/Register`. 

2. Change the user type of the registered customer to admin (UserType: 0). You can do this by updating the user's profile.

## Creating a Teller Account

After creating an admin account, you can create a teller account using the admin credentials. 

1. Log in as an admin to obtain an authentication token.

2. Make a POST request to `/api/Admin/AddTeller` to create a teller account. Use the admin's token for authentication.

## Creating a Customer Account

You can create a customer account using either the teller or registration process:

### Using Teller (with Teller Token)

1. Log in as a teller to obtain an authentication token.

2. Make a POST request to `/api/Teller/AddCustomer` to create a customer account. Use the teller's token for authentication.

### Using Registration

1. Make a POST request to `/api/Customer/Register` to register a customer account. This is available for self-registration.

## Account Types

You can create various types of accounts, such as Savings, Checking, etc., after customer registration.

## Teller Actions

Teller accounts have the following actions available:

- Deposit
- AddCustomer
- OpenAccount
- TransferMoney
- WithdrawCash
- BalanceInquiry

## Customer Actions

Customer accounts have the following actions available:

- TransferMoney
- BalanceInquiry

## Admin Actions

Admin accounts have the following actions available:

- AddTeller
