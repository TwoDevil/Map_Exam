CREATE TABLE MARK_CAR(
ID NUMBER NOT NULL PRIMARY KEY,
NAME NVARCHAR2(100) NOT NULL
);
CREATE TABLE CAR(
ID NUMBER NOT NULL PRIMARY KEY,
NAME_MODEL NVARCHAR2(100) NOT NULL,
COLOR NVARCHAR2(100) NOT NULL,
ID_MARK_CAR NUMBER NOT NULL,
PRICE NUMBER NOT NULL,
CONSTRAINT FK_MARK_CAR FOREIGN KEY(ID_MARK_CAR) REFERENCES MARK_CAR(ID)
);
CREATE TABLE LOGGER(
ID NUMBER NOT NULL PRIMARY KEY,
NAME_PLAYER NVARCHAR2(100) NOT NULL,
DATELOG DATE NOT NULL,
NAME_TYPE NVARCHAR2(100) NOT NULL,
NAME_TABLE NVARCHAR2(100) NOT NULL
);

CREATE SEQUENCE MARK_CAR_SEQ
  MINVALUE 1
  MAXVALUE 9999999
  START WITH 1
  INCREMENT BY 1
  CACHE 20;
  CREATE SEQUENCE CAR_SEQ
  MINVALUE 1
  MAXVALUE 9999999
  START WITH 1
  INCREMENT BY 1
  CACHE 20;
    CREATE SEQUENCE LOGGER_SEQ
  MINVALUE 1
  MAXVALUE 9999999
  START WITH 1
  INCREMENT BY 1
  CACHE 20;

  INSERT INTO MARK_CAR(NAME) VALUES('BMV2');
  INSERT INTO MARK_CAR(NAME) VALUES('UMS');
  INSERT INTO CAR(NAME_MODEL,COLOR,ID_MARK_CAR, PRICE) VALUES('X5','������',1,100000);
  INSERT INTO CAR(NAME_MODEL,COLOR,ID_MARK_CAR, PRICE) VALUES('X10','�����',1,10000);
  
  CREATE VIEW GETCAR AS SELECT NAME,NAME_MODEL,COLOR,PRICE FROM CAR INNER JOIN MARK_CAR ON CAR.ID_MARK_CAR=MARK_CAR.ID;


CREATE OR REPLACE TRIGGER ALLWORKMARKCAR
  BEFORE
    INSERT OR
    UPDATE OR
    DELETE ON MARK_CAR
    DECLARE
    v_DUAL NVARCHAR2(100);
BEGIN
SELECT ora_login_user INTO v_DUAL FROM DUAL;
  CASE
    WHEN INSERTING THEN
      DBMS_OUTPUT.PUT_LINE('Inserting');
      INSERT INTO LOGGER(ID, NAME_PLAYER,DATELOG,NAME_TYPE,NAME_TABLE) VALUES(LOGGER_SEQ.NEXTVAL, v_DUAL,TO_DATE(sysdate,'dd/mm/yyyy hh:mi:ss'),'INSERT','MARK_CAR');
    WHEN UPDATING THEN
      DBMS_OUTPUT.PUT_LINE('Updating');
      INSERT INTO LOGGER(ID, NAME_PLAYER,DATELOG,NAME_TYPE,NAME_TABLE) VALUES(LOGGER_SEQ.NEXTVAL, v_DUAL,TO_DATE(sysdate,'dd/mm/yyyy hh:mi:ss'),'UPDATE','MARK_CAR');
    WHEN DELETING THEN
      DBMS_OUTPUT.PUT_LINE('Deleting');
      INSERT INTO LOGGER(ID, NAME_PLAYER,DATELOG,NAME_TYPE,NAME_TABLE) VALUES(LOGGER_SEQ.NEXTVAL, v_DUAL,TO_DATE(sysdate,'dd/mm/yyyy hh:mi:ss'),'DELETE','MARK_CAR');
  END CASE;
END;
CREATE OR REPLACE TRIGGER ALLWORKCAR
  BEFORE
    INSERT OR
    UPDATE OR
    DELETE ON CAR
    DECLARE
    v_DUAL NVARCHAR2(100);
BEGIN
SELECT ora_login_user INTO v_DUAL FROM DUAL;
  CASE
    WHEN INSERTING THEN
      DBMS_OUTPUT.PUT_LINE('Inserting');
      INSERT INTO LOGGER(ID, NAME_PLAYER,DATELOG,NAME_TYPE,NAME_TABLE) VALUES(LOGGER_SEQ.NEXTVAL, v_DUAL,TO_DATE(sysdate,'dd/mm/yyyy hh:mi:ss'),'INSERT','CAR');
    WHEN UPDATING THEN
      DBMS_OUTPUT.PUT_LINE('Updating');
      INSERT INTO LOGGER(ID, NAME_PLAYER,DATELOG,NAME_TYPE,NAME_TABLE) VALUES(LOGGER_SEQ.NEXTVAL, v_DUAL,TO_DATE(sysdate,'dd/mm/yyyy hh:mi:ss'),'UPDATE','CAR');
    WHEN DELETING THEN
      DBMS_OUTPUT.PUT_LINE('Deleting');
      INSERT INTO LOGGER(ID, NAME_PLAYER,DATELOG,NAME_TYPE,NAME_TABLE) VALUES(LOGGER_SEQ.NEXTVAL, v_DUAL,TO_DATE(sysdate,'dd/mm/yyyy hh:mi:ss'),'DELETE','CAR');
  END CASE;
END;

CREATE OR REPLACE TRIGGER TRIGERINSERTCAR
   BEFORE INSERT ON CAR
   FOR EACH ROW
   WHEN (NEW.ID IS NULL)
BEGIN
   SELECT CAR_SEQ.NEXTVAL INTO :NEW.id FROM DUAL;
END;
CREATE OR REPLACE TRIGGER TRIGERINSERTMARKCAR
   BEFORE INSERT ON MARK_CAR
   FOR EACH ROW
   WHEN (NEW.ID IS NULL)
BEGIN
   SELECT MARK_CAR_SEQ.NEXTVAL INTO :NEW.id FROM DUAL;
END;

CREATE OR REPLACE VIEW CAR_ADD_NEW AS
   SELECT NAME_MODEL, COLOR, CAR.ID, MARK_CAR.NAME, PRICE
   FROM MARK_CAR, CAR
   WHERE MARK_CAR.ID = CAR.ID_MARK_CAR;

CREATE OR REPLACE TRIGGER ALLWORKCAR_ADD_NEW
   INSTEAD OF INSERT ON CAR_ADD_NEW
   DECLARE
   v_ID_MARK_CAR NUMBER;
   BEGIN
    
    IF (:NEW.ID IS NULL) THEN
    
    INSERT INTO MARK_CAR (NAME) VALUES (:new.NAME);
    SELECT ID INTO v_ID_MARK_CAR FROM MARK_CAR WHERE rownum = 1 ORDER BY ID DESC;
    INSERT INTO CAR (NAME_MODEL, COLOR, ID_MARK_CAR, PRICE) VALUES (:new.NAME_MODEL, :new.COLOR, v_ID_MARK_CAR, :new.PRICE);
    
    ELSIF (:NEW.NAME IS NULL) THEN
    INSERT INTO CAR (NAME_MODEL, COLOR, ID_MARK_CAR, PRICE) VALUES (:new.NAME_MODEL, :new.COLOR, :new.ID, :new.PRICE);
    END IF;
   END ALLWORKCAR_ADD_NEW;
   
  INSERT INTO CAR_ADD_NEW (NAME_MODEL, COLOR, PRICE,NAME) VALUES ('�111', '�����', 80000, 'BMS');
  INSERT INTO CAR_ADD_NEW (NAME_MODEL, COLOR, PRICE, ID) VALUES ('�141', '�������', 90000, 1);
  
  SELECT * FROM CAR;
  SELECT * FROM MARK_CAR;
    