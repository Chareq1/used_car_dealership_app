--
-- PostgreSQL database dump
--

-- Dumped from database version 16.3
-- Dumped by pg_dump version 16.3

-- Started on 2025-03-02 21:04:12 CET

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

DROP DATABASE IF EXISTS ucda;
--
-- TOC entry 4553 (class 1262 OID 16425)
-- Name: ucda; Type: DATABASE; Schema: -; Owner: -
--

CREATE DATABASE ucda WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'pl_PL.UTF-8';


\connect ucda

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 6 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA public;


--
-- TOC entry 4554 (class 0 OID 0)
-- Dependencies: 6
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON SCHEMA public IS 'standard public schema';


--
-- TOC entry 923 (class 1247 OID 16537)
-- Name: usertype; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.usertype AS ENUM (
    'ADMIN',
    'USER',
    'ADMINISTRATOR',
    'WORKER'
);


--
-- TOC entry 926 (class 1247 OID 16542)
-- Name: vehicletype; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.vehicletype AS ENUM (
    'Samochód',
    'Motocykl',
    'Dostawczy',
    'Ciągnik',
    'Autobus',
    'Ciężarówka',
    'Naczepa'
);


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 224 (class 1259 OID 16502)
-- Name: customers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.customers (
    "customerId" uuid NOT NULL,
    name character varying(255),
    surname character varying(255),
    pesel character varying(11),
    "idCardNumber" character varying(9),
    phone character varying(9),
    email character varying(255),
    "zipCode" character varying(6),
    city character varying(255),
    street character varying(255),
    "houseNumber" character varying(10)
);


--
-- TOC entry 219 (class 1259 OID 16446)
-- Name: documents; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.documents (
    "documentId" uuid NOT NULL,
    description text,
    file text,
    "customerId" uuid,
    "userId" uuid,
    "vehicleId" uuid,
    "creationDate" date
);


--
-- TOC entry 222 (class 1259 OID 16477)
-- Name: equipmentList; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."equipmentList" (
    "equipmentId" uuid NOT NULL,
    "vehicleId" uuid NOT NULL
);


--
-- TOC entry 221 (class 1259 OID 16470)
-- Name: equipments; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.equipments (
    "equipmentId" uuid NOT NULL,
    name character varying
);


--
-- TOC entry 220 (class 1259 OID 16458)
-- Name: images; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.images (
    "imageId" uuid NOT NULL,
    "fileName" character varying(255),
    "filePath" text,
    "vehicleId" uuid NOT NULL
);


--
-- TOC entry 218 (class 1259 OID 16433)
-- Name: locations; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.locations (
    "locationId" uuid NOT NULL,
    name character varying(255),
    phone character varying(9),
    email character varying(255),
    "zipCode" character varying(6),
    city character varying(255),
    street character varying(255),
    "houseNumber" character varying(10)
);


--
-- TOC entry 225 (class 1259 OID 16509)
-- Name: meetings; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.meetings (
    "meetingId" uuid NOT NULL,
    description text,
    "userId" uuid NOT NULL,
    "customerId" uuid,
    "locationId" uuid NOT NULL,
    date timestamp without time zone
);


--
-- TOC entry 223 (class 1259 OID 16495)
-- Name: users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.users (
    "userId" uuid NOT NULL,
    username character varying(255),
    password character varying(255),
    name character varying(255),
    surname character varying(255),
    pesel character varying(11),
    phone character varying(9),
    email character varying(255),
    "zipCode" character varying(6),
    city character varying(255),
    street character varying(255),
    "houseNumber" character varying(10),
    type public.usertype
);


--
-- TOC entry 217 (class 1259 OID 16426)
-- Name: vehicles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.vehicles (
    "vehicleId" uuid NOT NULL,
    brand character varying(255),
    model character varying(255),
    "bodyType" character varying(255),
    "productionYear" integer,
    "productionCountry" character varying(255),
    "firstRegistrationDate" date,
    "originCountry" character varying(255),
    mileage integer,
    doors integer,
    color character varying(255),
    transmission character varying(255),
    "VIN" character varying(17),
    description text,
    drive character varying(255),
    price double precision,
    "engineType" character varying(255),
    "fuelType" character varying(255),
    "engineSize" integer,
    power integer,
    consumption double precision,
    "electricEnginePower" double precision,
    "batterySize" double precision,
    "locationId" uuid NOT NULL,
    type public.vehicletype,
    "co2Emission" character varying(6)
);


--
-- TOC entry 4546 (class 0 OID 16502)
-- Dependencies: 224
-- Data for Name: customers; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.customers VALUES ('e75fc36b-0949-4732-b045-ffb2aeda61be', 'Jacek', 'Dorociński', '98122012913', 'DAC757823', '783795738', 'jacek.dorociński@wp.pl', '85-001', 'Bydgoszcz', 'Kruszwicka', '10A');
INSERT INTO public.customers VALUES ('a68b41f3-824a-4350-a9d3-7c02f354bf27', 'Janina', 'Smordzińska', '67122931627', 'DAY478214', '738795737', 'jan.smo@wp.pl', '85-132', 'Bydgoszcz', 'Podhalańska', '20B');
INSERT INTO public.customers VALUES ('b787c48a-2ddc-4b07-b7a2-cf7921eb03cb', 'Robert', 'Brązowy', '92110546576', 'DAK294827', '456123789', 'robert.braz@gmail.com', '85-634', 'Bydgoszcz', 'Józefa Sułkowskiego', '15C');


--
-- TOC entry 4541 (class 0 OID 16446)
-- Dependencies: 219
-- Data for Name: documents; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.documents VALUES ('45c8cd03-ac4c-46f4-be76-14d15e35db22', 'Cennik i konfigurator', '/home/chareq/ucda/ucda_documents/Cennik-Picanto_MY25_RP24_27-11-2024.pdf', 'a68b41f3-824a-4350-a9d3-7c02f354bf27', '83076c76-5c85-42a8-ab71-3786f7b463bd', '817ab5ee-06d9-4bc3-964d-4f5334beb336', '2025-01-03');
INSERT INTO public.documents VALUES ('ff70281a-1c63-421f-825a-a5225ee5c7f0', 'Instrukcja wykonania zdjęć do ubezpieczenia', '/home/chareq/ucda/ucda_documents/INSTRUKCJA-WYKONYWANIA-ZDJĘĆ-DO-AC (3) (1) (1).pdf', 'e75fc36b-0949-4732-b045-ffb2aeda61be', '83076c76-5c85-42a8-ab71-3786f7b463bd', '40d64ced-c13c-477c-9d42-f6abc4a69237', '2025-01-04');


--
-- TOC entry 4544 (class 0 OID 16477)
-- Dependencies: 222
-- Data for Name: equipmentList; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."equipmentList" VALUES ('de6a97b4-f05e-4581-bcbe-61c4c4b24b1e', '1948618b-45cb-4096-971e-a0f2bc0c35a6');
INSERT INTO public."equipmentList" VALUES ('e044b11e-02c1-4d15-a738-6f680048e930', '1948618b-45cb-4096-971e-a0f2bc0c35a6');
INSERT INTO public."equipmentList" VALUES ('349ab17e-615a-4026-88d7-0baa53753501', '1948618b-45cb-4096-971e-a0f2bc0c35a6');
INSERT INTO public."equipmentList" VALUES ('a39b2601-0eea-4b20-aa31-e9117571cdbb', '1948618b-45cb-4096-971e-a0f2bc0c35a6');


--
-- TOC entry 4543 (class 0 OID 16470)
-- Dependencies: 221
-- Data for Name: equipments; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.equipments VALUES ('a39b2601-0eea-4b20-aa31-e9117571cdbb', 'Klimatyzacja');
INSERT INTO public.equipments VALUES ('c9461f48-1843-4b81-8150-ca84089eb5dd', 'Radio');
INSERT INTO public.equipments VALUES ('b8b54e83-0847-4dc9-b086-e39db3e3b005', 'ABS');
INSERT INTO public.equipments VALUES ('19725f61-55ed-40b4-9d68-e4c95838cb56', 'Podgrzewane siedzenia');
INSERT INTO public.equipments VALUES ('5dbeb352-8125-4f2d-8bd7-6d4dd9fe6c17', 'Czujniki parkowania');
INSERT INTO public.equipments VALUES ('e272bf25-8a66-4d86-b440-ea93191b40f2', 'Tempomat');
INSERT INTO public.equipments VALUES ('411c476d-5439-466d-8064-eda0fef9fa67', 'Nawigacja');
INSERT INTO public.equipments VALUES ('4375ea40-ddc3-4bb0-a9ab-3d9df85bcd85', 'Elektryczne lusterka');
INSERT INTO public.equipments VALUES ('870be706-1c17-47b7-84f9-eff51f4a8548', 'Światła LED');
INSERT INTO public.equipments VALUES ('e0fa3ea9-5896-41d6-b50b-4f5a15405d4d', 'System audio premium');
INSERT INTO public.equipments VALUES ('eefa718f-ad26-4c18-8f3b-e67472e282cd', 'Kamera cofania');
INSERT INTO public.equipments VALUES ('97774c13-070e-46e8-b7e0-60eac4aabf4b', 'Poduszki powietrzne');
INSERT INTO public.equipments VALUES ('cc0054f2-4aa2-4c5c-b606-9f7712c00491', 'Skórzana tapicerka');
INSERT INTO public.equipments VALUES ('05951e37-5801-4fea-a1fd-01cad79e6a99', 'Ogrzewana szyba');
INSERT INTO public.equipments VALUES ('de6a97b4-f05e-4581-bcbe-61c4c4b24b1e', 'System bezkluczykowy');
INSERT INTO public.equipments VALUES ('4dbd7e97-5d3e-4199-9625-26ccf946509b', 'Felgi aluminiowe');
INSERT INTO public.equipments VALUES ('e044b11e-02c1-4d15-a738-6f680048e930', 'Elektryczne szyby');
INSERT INTO public.equipments VALUES ('fed98dd5-a588-46c4-8b9e-733ba21f59e1', 'Bluetooth');
INSERT INTO public.equipments VALUES ('292bc7b7-865e-4df9-b691-7e958d5c5aff', 'Regulacja wysokości siedzeń');
INSERT INTO public.equipments VALUES ('56b9853b-eaa0-496a-b5aa-e3bacfe3d1fa', 'Asystent pasa ruchu');
INSERT INTO public.equipments VALUES ('33621720-6530-4a94-aeb7-3910df5fa0ba', 'Ładowarka USB');
INSERT INTO public.equipments VALUES ('f9ea4ac9-7363-474e-8bdf-6c80138d872b', 'Fotele sportowe');
INSERT INTO public.equipments VALUES ('6568470b-1f22-4883-b4f5-94892c15ac92', 'System Start-Stop');
INSERT INTO public.equipments VALUES ('f07819bd-1fc9-4a11-a884-87d6338faade', 'Ogrzewanie postojowe');
INSERT INTO public.equipments VALUES ('bc3f1451-3213-4591-8ac8-86dbe543bd34', 'Hak holowniczy');
INSERT INTO public.equipments VALUES ('eca7302d-ed11-457d-a939-568b5d204cd6', 'System kontroli trakcji');
INSERT INTO public.equipments VALUES ('8fe9006c-9951-49d6-ac08-224b13b5775d', 'Alarm');
INSERT INTO public.equipments VALUES ('46b977cb-b5e9-4875-a05e-743e46d9c678', 'Panoramiczny dach');
INSERT INTO public.equipments VALUES ('a5f4d4a7-1797-4813-b0c2-0421d8a678c0', 'Podgrzewana kierownica');
INSERT INTO public.equipments VALUES ('b5736df3-48a3-4bee-b76b-91f18b7f1cea', 'Elektryczny bagażnik');
INSERT INTO public.equipments VALUES ('a50ea4ba-2aad-4a20-beae-1f27092e0968', 'Isofix');
INSERT INTO public.equipments VALUES ('a8c91240-5687-4636-9cb0-e73efe24be41', 'Reflektory adaptacyjne');
INSERT INTO public.equipments VALUES ('6dfba70e-10cb-4f77-85d2-60caede9e26f', 'Asystent parkowania');
INSERT INTO public.equipments VALUES ('e13b487f-0f68-49c8-ae55-066cdbe05ed9', 'System rozpoznawania znaków');
INSERT INTO public.equipments VALUES ('5920a54a-c130-4924-ba90-1a99dea5863a', 'Elektryczne siedzenia');
INSERT INTO public.equipments VALUES ('0ab6af90-d0e8-4ddc-a0aa-57675577688d', 'System monitorowania martwego pola');
INSERT INTO public.equipments VALUES ('b0132652-2aa2-4469-a213-6ebd0911acc3', 'Dach otwierany');
INSERT INTO public.equipments VALUES ('f474bf62-a82b-4e9d-b5fa-fa4b35f66820', 'System kontroli ciśnienia w oponach');
INSERT INTO public.equipments VALUES ('214aaa12-0322-4f5f-8360-63fd54435d8c', 'Zestaw głośnomówiący');
INSERT INTO public.equipments VALUES ('63e9a717-bb5f-4217-922c-15b497bcc882', 'Wyświetlacz HUD');
INSERT INTO public.equipments VALUES ('9954137d-6628-4909-9f76-5ba643eb09ca', 'Oświetlenie ambientowe');
INSERT INTO public.equipments VALUES ('e85a9a6a-b9e0-4f02-9400-e4725e3d73b0', 'Regulowana kolumna kierownicy');
INSERT INTO public.equipments VALUES ('c563d0ef-49ac-43ab-85cd-d74d530772a4', 'Przyciemniane szyby');
INSERT INTO public.equipments VALUES ('652077dd-932c-4506-a607-f102967057f4', 'Automatyczne wycieraczki');
INSERT INTO public.equipments VALUES ('260b2cfb-e58b-454a-a56c-1034f10f666d', 'Chłodzenie w fotelach');
INSERT INTO public.equipments VALUES ('1f9dc004-1b0d-4e0f-b7f5-87d9938e4880', 'Uchwyty na kubki');
INSERT INTO public.equipments VALUES ('e487ddd8-a4fa-483b-8669-acb4c527dd70', 'Wielofunkcyjna kierownica');
INSERT INTO public.equipments VALUES ('349ab17e-615a-4026-88d7-0baa53753501', 'Czujnik deszczu');
INSERT INTO public.equipments VALUES ('6913bc23-bbd9-4a77-a905-858582e77b74', 'System ostrzegania przed kolizją');
INSERT INTO public.equipments VALUES ('6eac526f-f7f4-441b-9db3-0974a4a5d5ab', 'Podgrzewane lusterka');
INSERT INTO public.equipments VALUES ('c37e3007-26a9-41d2-9be6-5cf1e048f80b', 'Elektryczny hamulec postojowy');
INSERT INTO public.equipments VALUES ('93ddab10-512a-4751-9b3b-6b5bda391112', 'System rozrywki dla pasażerów');
INSERT INTO public.equipments VALUES ('3b279910-f54d-4dbe-a7fc-fa045ccea057', 'Oświetlenie drogi do domu');
INSERT INTO public.equipments VALUES ('a3c29553-d584-4fb4-af4e-555838f71f21', 'System antykradzieżowy');
INSERT INTO public.equipments VALUES ('304d5bf9-1d90-49bf-aa55-acffe90e04a6', 'Składane tylne siedzenia');
INSERT INTO public.equipments VALUES ('c4557fe3-2273-4362-89da-d95f79f18ab0', 'Adaptacyjny tempomat');
INSERT INTO public.equipments VALUES ('f3693213-2915-4f53-83b7-2dd0151b4576', 'System rozpoznawania zmęczenia kierowcy');
INSERT INTO public.equipments VALUES ('14ea8881-4d07-440a-9156-12942eed12f8', 'Ładowarka indukcyjna');
INSERT INTO public.equipments VALUES ('067bd40d-2066-405c-8811-4cfa581c64fa', 'System śledzenia GPS');
INSERT INTO public.equipments VALUES ('fd5da374-2e34-44d4-966c-397d7d36c642', 'Chromowane elementy wykończenia');
INSERT INTO public.equipments VALUES ('470ea3fa-7baa-41fe-96ce-96026798deb0', 'Zestaw naprawczy do opon');
INSERT INTO public.equipments VALUES ('341db8d9-a8ef-4d92-8d79-a6c6a8f10879', 'Osłony przeciwsłoneczne');
INSERT INTO public.equipments VALUES ('add8dd4c-01e2-4dce-a546-8ad0bb9e7cd2', 'Regulowane zawieszenie');
INSERT INTO public.equipments VALUES ('2f669f62-88df-4ff7-95d6-d43659a5e99c', 'System awaryjnego hamowania');
INSERT INTO public.equipments VALUES ('9717be42-cbdc-4d80-8ed5-dbd800edeeaf', 'System wspomagania ruszania pod górę');
INSERT INTO public.equipments VALUES ('702e912f-cefc-48d1-ab83-7e22e97775d8', 'Oświetlenie wewnętrzne LED');
INSERT INTO public.equipments VALUES ('232c9145-79a2-45a1-b094-6b8dd711e9eb', 'Dywaniki welurowe');
INSERT INTO public.equipments VALUES ('5e92c41e-28cf-4f16-a320-82df4d01ef2c', 'Przestrzeń bagażowa z organizerem');
INSERT INTO public.equipments VALUES ('4f62360c-c6e1-4eed-9c83-256f7e390487', 'Elektryczna roleta tylna');
INSERT INTO public.equipments VALUES ('0a2a04f6-c11c-464f-9fac-6398f185a58d', 'Czujniki zmierzchu');
INSERT INTO public.equipments VALUES ('97641026-0c03-49d8-9f7e-1dbba130c004', 'System wykrywania pieszych');
INSERT INTO public.equipments VALUES ('813b0274-7412-4c98-8709-d02d52f19413', 'System eCall');
INSERT INTO public.equipments VALUES ('93ee4c40-f956-43b7-beb9-460c54db33dc', 'Fotelik dziecięcy');
INSERT INTO public.equipments VALUES ('488572f5-e730-4ceb-a235-b5fef3633086', 'Pompka elektryczna');
INSERT INTO public.equipments VALUES ('d59d5f2b-47bc-42f8-a2cc-b901ca8c4560', 'System ogranicznika prędkości');
INSERT INTO public.equipments VALUES ('fc6677d4-e7d2-4eae-80cf-13cfe2b0254b', 'Przenośny odkurzacz samochodowy');
INSERT INTO public.equipments VALUES ('8b024a11-3fd7-4b41-8aac-7e47b5578dc1', 'Osłony na szyby przeciwsłoneczne');
INSERT INTO public.equipments VALUES ('d32b8ddf-c405-485e-895c-1abe688297bc', 'Schowek na okulary');
INSERT INTO public.equipments VALUES ('1690dba5-d02d-4549-a4d6-626c49d78558', 'Uchwyty na rowery');
INSERT INTO public.equipments VALUES ('a5da809b-2031-46bb-8165-809f0b2c95dc', 'Bagażnik dachowy');
INSERT INTO public.equipments VALUES ('543b2d21-71e1-43c1-acc4-fa5dc7023c57', 'Podświetlenie klamek');
INSERT INTO public.equipments VALUES ('fdde44b0-10aa-4bfa-8078-cff8993b3ec8', 'Elektryczne fotele tylne');
INSERT INTO public.equipments VALUES ('4603c0b6-5b6c-4013-95fa-25927f291185', 'Podgrzewane tylne siedzenia');
INSERT INTO public.equipments VALUES ('d72fffff-f055-4cfc-9f19-5903d1360c04', 'Składane lusterka boczne');
INSERT INTO public.equipments VALUES ('95a69a5e-e3d4-488b-ae65-d0fdc4467fb5', 'System wspomagania zjazdu');
INSERT INTO public.equipments VALUES ('d18d3db3-3285-4b5d-9dce-92e87f5369ed', 'System akustyczny ostrzegający pieszych');
INSERT INTO public.equipments VALUES ('5e0c13e0-0faf-4229-9374-8cc47e3f6c9d', 'Aktywny asystent parkowania');
INSERT INTO public.equipments VALUES ('6bda3a59-04a2-400d-ad64-490c216c7828', 'Asystent jazdy nocnej');
INSERT INTO public.equipments VALUES ('bb932ebd-cf0d-471d-861e-077194d0500c', 'System monitorowania ciśnienia w oponach');
INSERT INTO public.equipments VALUES ('84f017a7-07b1-418a-b653-ec13941ed17b', 'Elektryczny kluczyk');
INSERT INTO public.equipments VALUES ('813c3fb3-8ce6-461a-b035-879905ce96c1', 'Zewnętrzna antena radiowa');
INSERT INTO public.equipments VALUES ('608b3289-698d-4f1b-8ae1-7e7ef417d314', 'System podgrzewania napojów');
INSERT INTO public.equipments VALUES ('ce6608b9-aa5d-447a-8278-542995c6d368', 'Dodatkowy schowek w podłokietniku');
INSERT INTO public.equipments VALUES ('96b08f73-bed8-42d0-8c47-bcf2c952c8eb', 'Składane stoliki');
INSERT INTO public.equipments VALUES ('d772be80-ca86-4e9a-ae89-db332fc72b88', 'Roleta przeciwsłoneczna tylnej szyby');
INSERT INTO public.equipments VALUES ('0a1120fc-2ae8-4ba3-9aaa-b6d4709bcd2b', 'Filtr zapachów w kabinie');
INSERT INTO public.equipments VALUES ('2bf63f1e-b53e-4e83-b870-f864e9d6ebba', 'System automatycznego parkowania');
INSERT INTO public.equipments VALUES ('d8d39dd5-8621-4375-ba30-eac2a030b672', 'System nagłośnienia przestrzennego');
INSERT INTO public.equipments VALUES ('8c16134f-6311-498b-95e3-6473d40d4fd2', 'Kieszenie w tylnych drzwiach');
INSERT INTO public.equipments VALUES ('8b9e612e-d495-45ee-b404-f2b414411cd9', 'Podświetlane progi boczne');
INSERT INTO public.equipments VALUES ('40512d96-f11f-4d0a-b123-d1fc954a2844', 'System monitorowania obiektów z tyłu');
INSERT INTO public.equipments VALUES ('074147c3-7a33-4847-a570-3d0df32550e4', 'System automatycznego domykania drzwi');


--
-- TOC entry 4542 (class 0 OID 16458)
-- Dependencies: 220
-- Data for Name: images; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.images VALUES ('8b3f97cc-fa86-421e-b78b-d4020ccd3654', '1_1.jpg', '/home/chareq/ucda/ucda_images/fiat_panda_ZFA31200003635586/', '40d64ced-c13c-477c-9d42-f6abc4a69237');
INSERT INTO public.images VALUES ('7a4972a0-74be-4656-88f2-ffce2c7b236b', '1.jpg', '/home/chareq/ucda/ucda_images/tesla_model 3_5YJ3E7EB9KF338731/', '1948618b-45cb-4096-971e-a0f2bc0c35a6');
INSERT INTO public.images VALUES ('5b90eb83-6854-4120-b0ae-e8057e95d486', 'VW_Crafter_2.0_TDI_(Facelift)_–_Frontansicht,_9._Juli_2012,_Velbert.jpg', '/home/chareq/ucda/ucda_images/volkswagen_crafter_WV1ZZZ2EZD6003752/', '817ab5ee-06d9-4bc3-964d-4f5334beb336');
INSERT INTO public.images VALUES ('83ad43ec-2b7c-44fa-a5b6-e2cfafabd6c3', 'truck-tractor-MAN-TGX-18-460---1689663609234692289_big--23071211115260480300.jpg', '/home/chareq/ucda/ucda_images/man_tgx 18.460_WMA06XZZ0KP116160/', '85208109-df7b-4a77-9014-4c65c9f6ca6c');


--
-- TOC entry 4540 (class 0 OID 16433)
-- Dependencies: 218
-- Data for Name: locations; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.locations VALUES ('883ed7c5-32f1-4cef-9e4a-7d595f0b927c', 'Nad Górą', '509276551', 'kontakt@nadgora.pl', '86-005', 'Białe Błota', 'Słoneczna', '23');
INSERT INTO public.locations VALUES ('7cd0270a-356b-49ae-bdfb-eadda88bd063', 'Unique Cars', '645120037', 'unique@wp.pl', '86-006', 'Łochowo', 'Polerska', '303C');
INSERT INTO public.locations VALUES ('598b5110-69a7-4322-8fd7-f44bba092210', 'Strong Cars', '886234256', 'info@strongcars.pl', '85-125', 'Bydgoszcz', 'Solskiego', '52A');


--
-- TOC entry 4547 (class 0 OID 16509)
-- Dependencies: 225
-- Data for Name: meetings; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.meetings VALUES ('d6fd3955-e195-4d2c-9b58-b75a6d598c84', 'Initial consultation', '5ce8102d-b8ac-4ef3-9c5a-cbd25ac279ad', 'e75fc36b-0949-4732-b045-ffb2aeda61be', '883ed7c5-32f1-4cef-9e4a-7d595f0b927c', '2024-01-21 17:00:00');
INSERT INTO public.meetings VALUES ('b097dbb2-5e4e-49f2-b2f3-add91064879f', 'Performance review', '83076c76-5c85-42a8-ab71-3786f7b463bd', 'a68b41f3-824a-4350-a9d3-7c02f354bf27', '7cd0270a-356b-49ae-bdfb-eadda88bd063', '2024-01-21 13:00:00');
INSERT INTO public.meetings VALUES ('211ee72d-0f0e-4721-9380-13a3465eda7b', 'Testowe spotkanie z klientem', '83076c76-5c85-42a8-ab71-3786f7b463bd', 'b787c48a-2ddc-4b07-b7a2-cf7921eb03cb', '7cd0270a-356b-49ae-bdfb-eadda88bd063', '2024-01-20 18:00:00');
INSERT INTO public.meetings VALUES ('b5469444-43db-47f0-a8b1-a9816d7803fa', 'Test123', '83076c76-5c85-42a8-ab71-3786f7b463bd', 'a68b41f3-824a-4350-a9d3-7c02f354bf27', '598b5110-69a7-4322-8fd7-f44bba092210', '2024-03-27 03:03:00');
INSERT INTO public.meetings VALUES ('d7922e3b-6c77-4a25-9684-76e5c56434b8', 'uewhofejwiofmeikmfef', '83076c76-5c85-42a8-ab71-3786f7b463bd', 'e75fc36b-0949-4732-b045-ffb2aeda61be', '7cd0270a-356b-49ae-bdfb-eadda88bd063', '2024-12-01 20:30:00');
INSERT INTO public.meetings VALUES ('440a1fcf-d4e2-48c1-8c15-2d697acb658c', 'Omówienie planu ratowego', '83076c76-5c85-42a8-ab71-3786f7b463bd', 'e75fc36b-0949-4732-b045-ffb2aeda61be', '7cd0270a-356b-49ae-bdfb-eadda88bd063', '2024-12-30 17:00:00');
INSERT INTO public.meetings VALUES ('9de0f506-086d-4812-b010-45a2d7455d2b', 'Pokazanie samochodów', '83076c76-5c85-42a8-ab71-3786f7b463bd', 'e75fc36b-0949-4732-b045-ffb2aeda61be', '883ed7c5-32f1-4cef-9e4a-7d595f0b927c', '2024-01-20 13:00:00');
INSERT INTO public.meetings VALUES ('a8ef8406-3aed-4b09-813e-0412e2c4fed6', 'Testowe spotkanie z klientem #2', '83076c76-5c85-42a8-ab71-3786f7b463bd', 'e75fc36b-0949-4732-b045-ffb2aeda61be', '7cd0270a-356b-49ae-bdfb-eadda88bd063', '2025-01-04 10:25:00');


--
-- TOC entry 4545 (class 0 OID 16495)
-- Dependencies: 223
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.users VALUES ('6e321a82-f5c9-49b0-95e0-62821d1421c8', 'asmith', 'emFxMUBXU1g=', 'Alicja', 'Smith', '88091447843', '832957924', 'alicja.smith@nadgora.pl', '86-005', 'Białe Błota', 'Podolska', '202B', 'WORKER');
INSERT INTO public.users VALUES ('c6778e34-2ca5-442e-95f2-090260ee9f43', 'testowyjanusz', 'VGVzdG93eTEyMyM=', 'Janusz', 'Testowy', '66061332492', '641728364', 'jantest@gmail.com', '78-005', 'Lucinek', 'Wileńska', '10', 'ADMINISTRATOR');
INSERT INTO public.users VALUES ('5ce8102d-b8ac-4ef3-9c5a-cbd25ac279ad', 'jdoe', 'SmFjRG9lMTIzIw==', 'Jacek', 'Doe', '92030469171', '463748378', 'jacek.doe@strongcars.pl', '88-110', 'Inowrocław', 'Podczaska', '101A', 'WORKER');
INSERT INTO public.users VALUES ('83076c76-5c85-42a8-ab71-3786f7b463bd', '123chr', 'Q3phcmVrMDgj', 'Kacper', 'Owczarek', '02310802538', '483174921', 'kacowc@wp.pl', '86-005', 'Białe Błota', 'Lechicka', '10', 'ADMINISTRATOR');


--
-- TOC entry 4539 (class 0 OID 16426)
-- Dependencies: 217
-- Data for Name: vehicles; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.vehicles VALUES ('85208109-df7b-4a77-9014-4c65c9f6ca6c', 'MAN', 'TGX 18.460', 'Skrzyniowe', 2018, 'Wielka Brytania', '2018-04-07', 'Polska', 446000, 2, 'Biały', 'Automatyczna', 'WMA06XZZ0KP116160', '---', '4WD', 94710.5, 'Spalinowy', 'Diesel', 12000, 460, 10.9, NULL, NULL, '7cd0270a-356b-49ae-bdfb-eadda88bd063', 'Ciężarówka', 'EURO5');
INSERT INTO public.vehicles VALUES ('1948618b-45cb-4096-971e-a0f2bc0c35a6', 'Tesla', 'Model 3', 'Sedan', 2017, 'USA', '2017-08-10', 'Niemcy', 56789, 5, 'Biały', 'Automatyczna', '5YJ3E7EB9KF338731', '---', 'AWD', 150000, 'Elektryczny', '', 0, 0, 19, 480, 78, '7cd0270a-356b-49ae-bdfb-eadda88bd063', 'Samochód', NULL);
INSERT INTO public.vehicles VALUES ('40d64ced-c13c-477c-9d42-f6abc4a69237', 'Fiat', 'Panda', 'Hatchback', 2015, 'Włochy', '2016-03-08', 'Niemcy', 105625, 6, 'Czarny', 'Manualna', 'ZFA31200003635586', '---', 'FWD', 25000.5, 'Spalinowy', 'Benzyna', 1200, 75, 7.9, 0, 0, '7cd0270a-356b-49ae-bdfb-eadda88bd063', 'Samochód', 'EURO6');
INSERT INTO public.vehicles VALUES ('817ab5ee-06d9-4bc3-964d-4f5334beb336', 'Volkswagen', 'Crafter', 'Minivan', 2012, 'Dominika', '2012-10-06', 'Polska', 377350, 4, 'Czarny', 'Manualna', 'WV1ZZZ2EZD6003752', 'Dzień dobry,jwhdihwudhuwhduwhuhduhqlihdqiwjodkjninwmsnsnwjsnjwnsiwnoiqjhoiejd9-weuipo1321216271638716274627637267382xb7ew2t7346726378267318937193718738617643726481678468', 'AWD', 135000, 'Spalinowy', 'Diesel', 1968, 163, 8.7, NULL, NULL, '883ed7c5-32f1-4cef-9e4a-7d595f0b927c', 'Samochód', 'EURO5');


--
-- TOC entry 4377 (class 2606 OID 16610)
-- Name: customers customers_email_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.customers
    ADD CONSTRAINT customers_email_key UNIQUE (email);


--
-- TOC entry 4379 (class 2606 OID 16608)
-- Name: customers customers_idCardNumber_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.customers
    ADD CONSTRAINT "customers_idCardNumber_key" UNIQUE ("idCardNumber");


--
-- TOC entry 4381 (class 2606 OID 16606)
-- Name: customers customers_pesel_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.customers
    ADD CONSTRAINT customers_pesel_key UNIQUE (pesel);


--
-- TOC entry 4383 (class 2606 OID 16508)
-- Name: customers customers_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.customers
    ADD CONSTRAINT customers_pkey PRIMARY KEY ("customerId");


--
-- TOC entry 4363 (class 2606 OID 16452)
-- Name: documents documents_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents
    ADD CONSTRAINT documents_pkey PRIMARY KEY ("documentId");


--
-- TOC entry 4367 (class 2606 OID 16476)
-- Name: equipments equipments_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.equipments
    ADD CONSTRAINT equipments_pkey PRIMARY KEY ("equipmentId");


--
-- TOC entry 4365 (class 2606 OID 16464)
-- Name: images images_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.images
    ADD CONSTRAINT images_pkey PRIMARY KEY ("imageId");


--
-- TOC entry 4359 (class 2606 OID 16604)
-- Name: locations locations_email_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.locations
    ADD CONSTRAINT locations_email_key UNIQUE (email);


--
-- TOC entry 4361 (class 2606 OID 16439)
-- Name: locations locations_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.locations
    ADD CONSTRAINT locations_pkey PRIMARY KEY ("locationId");


--
-- TOC entry 4385 (class 2606 OID 16515)
-- Name: meetings meetings_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.meetings
    ADD CONSTRAINT meetings_pkey PRIMARY KEY ("meetingId");


--
-- TOC entry 4369 (class 2606 OID 16600)
-- Name: users users_email_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- TOC entry 4371 (class 2606 OID 16598)
-- Name: users users_pesel_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pesel_key UNIQUE (pesel);


--
-- TOC entry 4373 (class 2606 OID 16501)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY ("userId");


--
-- TOC entry 4375 (class 2606 OID 16594)
-- Name: users users_username_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_username_key UNIQUE (username);


--
-- TOC entry 4355 (class 2606 OID 16602)
-- Name: vehicles vehicles_VIN_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.vehicles
    ADD CONSTRAINT "vehicles_VIN_key" UNIQUE ("VIN");


--
-- TOC entry 4357 (class 2606 OID 16432)
-- Name: vehicles vehicles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.vehicles
    ADD CONSTRAINT vehicles_pkey PRIMARY KEY ("vehicleId");


--
-- TOC entry 4353 (class 1259 OID 16445)
-- Name: fki_l; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX fki_l ON public.vehicles USING btree ("locationId");


--
-- TOC entry 4387 (class 2606 OID 16661)
-- Name: documents documents_customerId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents
    ADD CONSTRAINT "documents_customerId_fkey" FOREIGN KEY ("customerId") REFERENCES public.customers("customerId") ON DELETE SET NULL NOT VALID;


--
-- TOC entry 4388 (class 2606 OID 16666)
-- Name: documents documents_userId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents
    ADD CONSTRAINT "documents_userId_fkey" FOREIGN KEY ("userId") REFERENCES public.users("userId") ON DELETE SET NULL NOT VALID;


--
-- TOC entry 4389 (class 2606 OID 16671)
-- Name: documents documents_vehicleId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents
    ADD CONSTRAINT "documents_vehicleId_fkey" FOREIGN KEY ("vehicleId") REFERENCES public.vehicles("vehicleId") ON DELETE SET NULL NOT VALID;


--
-- TOC entry 4391 (class 2606 OID 16626)
-- Name: equipmentList equipmentList_equipmentId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."equipmentList"
    ADD CONSTRAINT "equipmentList_equipmentId_fkey" FOREIGN KEY ("equipmentId") REFERENCES public.equipments("equipmentId") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4392 (class 2606 OID 16631)
-- Name: equipmentList equipmentList_vehicleId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."equipmentList"
    ADD CONSTRAINT "equipmentList_vehicleId_fkey" FOREIGN KEY ("vehicleId") REFERENCES public.vehicles("vehicleId") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4390 (class 2606 OID 16636)
-- Name: images images_vehicleId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.images
    ADD CONSTRAINT "images_vehicleId_fkey" FOREIGN KEY ("vehicleId") REFERENCES public.vehicles("vehicleId") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4393 (class 2606 OID 16646)
-- Name: meetings meetings_customerId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.meetings
    ADD CONSTRAINT "meetings_customerId_fkey" FOREIGN KEY ("customerId") REFERENCES public.customers("customerId") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4394 (class 2606 OID 16651)
-- Name: meetings meetings_locationId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.meetings
    ADD CONSTRAINT "meetings_locationId_fkey" FOREIGN KEY ("locationId") REFERENCES public.locations("locationId") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4395 (class 2606 OID 16641)
-- Name: meetings meetings_userId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.meetings
    ADD CONSTRAINT "meetings_userId_fkey" FOREIGN KEY ("userId") REFERENCES public.users("userId") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4386 (class 2606 OID 16656)
-- Name: vehicles vehicles_locationId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.vehicles
    ADD CONSTRAINT "vehicles_locationId_fkey" FOREIGN KEY ("locationId") REFERENCES public.locations("locationId") ON DELETE SET NULL NOT VALID;


-- Completed on 2025-03-02 21:04:12 CET

--
-- PostgreSQL database dump complete
--

