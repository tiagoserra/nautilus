CREATE DATABASE nautilus
    WITH 
    OWNER = postgres
    TABLESPACE = pg_default
    CONNECTION LIMIT = 1000;

DROP ROLE IF EXISTS nautilus;

CREATE ROLE nautilus WITH 
	PASSWORD 'RGV1cyBzZWphIGxvdXZhZG8g'
	SUPERUSER
	CREATEDB
	CREATEROLE
	INHERIT
	LOGIN
	REPLICATION
	BYPASSRLS
	CONNECTION LIMIT -1;