USE master


IF DB_ID('tenmo_test') IS NOT NULL
BEGIN
	ALTER DATABASE tenmo_test SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE tenmo_test;
END

CREATE DATABASE tenmo_test


USE tenmo_test


CREATE TABLE transfer_types (
	transfer_type_id int IDENTITY(1,1) NOT NULL,
	transfer_type_desc varchar(10) NOT NULL,
	CONSTRAINT PK_transfer_types PRIMARY KEY (transfer_type_id)
)

CREATE TABLE transfer_statuses (
	transfer_status_id int IDENTITY(1,1) NOT NULL,
	transfer_status_desc varchar(10) NOT NULL,
	CONSTRAINT PK_transfer_statuses PRIMARY KEY (transfer_status_id)
)

CREATE TABLE users (
	user_id int IDENTITY(1,1) NOT NULL,
	username varchar(50) NOT NULL,
	password_hash varchar(200) NOT NULL,
	salt varchar(200) NOT NULL,
	CONSTRAINT PK_user PRIMARY KEY (user_id)
)

CREATE TABLE accounts (
	account_id int IDENTITY(1,1) NOT NULL,
	user_id int NOT NULL,
	balance decimal(13, 2) NOT NULL,
	CONSTRAINT PK_accounts PRIMARY KEY (account_id),
	CONSTRAINT FK_accounts_user FOREIGN KEY (user_id) REFERENCES users (user_id)
)

CREATE TABLE transfers (
	transfer_id int IDENTITY(1,1) NOT NULL,
	transfer_type_id int NOT NULL,
	transfer_status_id int NOT NULL,
	account_from int NOT NULL,
	account_to int NOT NULL,
	amount decimal(13, 2) NOT NULL,
	CONSTRAINT PK_transfers PRIMARY KEY (transfer_id),
	CONSTRAINT FK_transfers_accounts_from FOREIGN KEY (account_from) REFERENCES accounts (account_id),
	CONSTRAINT FK_transfers_accounts_to FOREIGN KEY (account_to) REFERENCES accounts (account_id),
	CONSTRAINT FK_transfers_transfer_statuses FOREIGN KEY (transfer_status_id) REFERENCES transfer_statuses (transfer_status_id),
	CONSTRAINT FK_transfers_transfer_types FOREIGN KEY (transfer_type_id) REFERENCES transfer_types (transfer_type_id),
	CONSTRAINT CK_transfers_not_same_account CHECK  ((account_from<>account_to)),
	CONSTRAINT CK_transfers_amount_gt_0 CHECK ((amount>0))
)


INSERT INTO transfer_statuses (transfer_status_desc) VALUES ('Pending');
INSERT INTO transfer_statuses (transfer_status_desc) VALUES ('Approved');
INSERT INTO transfer_statuses (transfer_status_desc) VALUES ('Rejected');

INSERT INTO transfer_types (transfer_type_desc) VALUES ('Request');
INSERT INTO transfer_types (transfer_type_desc) VALUES ('Send');

INSERT INTO users (username, password_hash, salt) VALUES
	('test1', 'test', 'test'),
	('test2', 'test', 'test'),
	('test3', 'test', 'test'),
	('test4', 'test', 'test'),
	('test5', 'test', 'test')

INSERT INTO accounts (user_id, balance) VALUES
	(1, 1000),
	(2, 1000),
	(3, 1000),
	(4, 1000),
	(5, 1000)

INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES
	(1, 1, 1, 2, 100),
	(1, 1, 1, 3, 10),
	(1, 1, 2, 1, 100),
	(2, 2, 1, 2, 50),
	(2, 2, 2, 1, 50),
	(1, 2, 1, 2, 25),
	(1, 3, 1, 2, 50),
	(2, 2, 3, 4, 25)

