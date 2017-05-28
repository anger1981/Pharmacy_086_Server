

									/*									*/
									/* CREATING PROCEDURES OF DRUGSTORE */
									/*									*/


									/*											*/
									/* Creating Procedure Of UpdatingDrugstores */
									/*											*/


CREATE DEFINER = 'root'@'localhost'
PROCEDURE pharm66.UpdatingDrugstores(IN ID INT, IN ID_DI INT, IN Name VARBINARY (255), IN Address VARBINARY (255), IN Phone VARBINARY (127), IN Mail VARBINARY (63), IN Site VARBINARY (127), IN Schedule VARBINARY (255), IN Transport VARBINARY (255), IN Deleting BINARY)
BEGIN

  IF (Deleting = 1) THEN

    UPDATE
      Pharmacy
    SET
      Is_deleted = 1
    WHERE
      (Id_Pharmacy = ID);

    DELETE
    FROM
      Price_List
    WHERE
      Id_Pharmacy = ID;

  ELSE

    IF (EXISTS (SELECT
      *
    FROM
      Pharmacy AS D
    WHERE
      (D.Id_Pharmacy = ID))) THEN

      UPDATE
        Pharmacy AS D
      SET
        D.Name_full = Name, D.Addr = Address, D.Phone = Phone, D.Mail = Mail, D.Web = Site, D.Hours = Schedule, D.Trans = Transport, D.Id_District = ID_DI, D.Is_deleted = Deleting
      WHERE
        (D.Id_Pharmacy = ID);

    ELSE

      INSERT
      INTO Pharmacy (Id_Pharmacy, Name_full, Addr, Phone, Mail, Web, Hours, Trans, Id_District, Is_deleted)
      VALUES (ID, Name, Address, Phone, Mail, Site, Schedule, Transport, ID_DI, Deleting);

    END IF;
  END IF;
END


									/*												  */
									/* Creating Procedure Of UpdatingGroupsOfProducts */
									/*												  */


CREATE DEFINER = 'root'@'localhost'
PROCEDURE pharm66.UpdatingGroupsOfProducts(IN ID INT, IN Name VARBINARY (255), IN DateOfUpdating DATETIME, IN Deleting BINARY)
BEGIN

  IF (Deleting = 1) THEN
    UPDATE
      Product_group
    SET
      Is_deleted = 1
    WHERE
      (Id_product_group = ID);

  ELSE

    IF (EXISTS (SELECT
      *
    FROM
      Product_group AS PG
    WHERE
      (PG.Id_product_group = ID))) THEN

      UPDATE
        Product_group AS PG
      SET
        PG.Id_product_group = ID, PG.Name_full = Name, PG.Date_upd = DateOfUpdating, PG.Is_deleted = Deleting
      WHERE
        (PG.Id_product_group = ID);

    ELSE

      INSERT
      INTO Product_group (Id_product_group, Name_full, Date_upd, Is_deleted)
      VALUES (ID, Name, DateOfUpdating, Deleting);

    END IF;
  END IF;
END


									/*											*/
									/* Creating Procedure Of UpdatingOfProducts */
									/*											*/


CREATE DEFINER = 'root'@'localhost'
PROCEDURE pharm66.UpdatingOfProducts(IN ID INT, IN ID_PG INT, IN Name VARBINARY (255), IN Composition VARBINARY (255), IN Description VARBINARY (65535), IN Updating DATETIME, IN Deleting BINARY)
BEGIN

  IF (Deleting = 1) THEN
    UPDATE
      Product
    SET
      Is_deleted = 1
    WHERE
      (Id_Product = ID);

  ELSE

    IF (EXISTS (SELECT
      *
    FROM
      Product AS P
    WHERE
      (P.Id_Product = ID))) THEN

      UPDATE
        Product AS P
      SET
        P.Id_product_group = ID_PG, P.Name_full = Name, P.Composition = Composition, P.Description = Description, P.Date_upd = Updating, P.Is_deleted = Deleting
      WHERE
        (P.Id_Product = ID);

    ELSE

      INSERT
      INTO Product (Id_Product, Id_product_group, Name_full, Composition, Description, Date_upd, Is_deleted)
      VALUES (ID, ID_PG, Name, Composition, Description, Updating, Deleting);

    END IF;
  END IF;
END


									/*											*/
									/* Creating Procedure Of UpdatingPriceList	*/
									/*											*/


CREATE DEFINER = 'root'@'localhost'
PROCEDURE pharm66.UpdatingPriceList(IN ID_PH INT, IN ID_PR INT, IN Price DECIMAL (12, 3), IN Updating DATETIME, IN Preferential BINARY, IN Deleting BINARY)
BEGIN

  IF (Deleting = 1) THEN

    DELETE
    FROM
      Price_List
    WHERE
      ((Id_Pharmacy = ID_PH) AND (Id_Product = ID_PR));

  ELSE

    IF (EXISTS (SELECT
      *
    FROM
      Price_List AS PL
    WHERE
      ((PL.Id_Pharmacy = ID_PH) AND (PL.Id_Product = ID_PR)))) THEN

      UPDATE
        Price_list AS PL
      SET
        PL.Price = Price, PL.Date_upd = Updating, PL.Is_privilege = Preferential, PL.Is_deleted = Deleting
      WHERE
        ((PL.Id_Pharmacy = ID_PH) AND (PL.Id_Product = ID_PR));

    ELSE

      INSERT
      INTO Price_list (Id_Pharmacy, ID_Product, Price, Date_upd, Is_privilege, Is_deleted)
      VALUES (ID_PH, ID_PR, Price, Updating, Preferential, Deleting);

    END IF;
  END IF;
END


									/*													*/
									/* Creating Procedure Of UpdatingDatesOfPriceList	*/
									/*													*/


CREATE DEFINER = 'root'@'localhost'
PROCEDURE pharm66.UpdatingDatesOfPriceList(IN ID INT, IN `Date` DATETIME)
BEGIN

  UPDATE
    Price_list AS PL
  SET
    PL.Date_upd = Date
  WHERE
    (PL.Id_Pharmacy = ID);

  UPDATE
    Pharmacy AS P
  SET
    P.Date_upd = Date
  WHERE
    (P.Id_Pharmacy = ID);

END


									/*													*/
									/* Creating Procedure Of UpdatingOfAnnouncements	*/
									/*													*/


/* Creating Table Announcements */
CREATE TABLE Pharm66.Announcements(
  ID_PH          INT (10)      NOT NULL,
  ID             INT (10)      NOT NULL,
  Caption        VARCHAR (255) NULL,
  Text           LONGTEXT      NULL,
  DateOfUpdating DATETIME      NULL,
  PRIMARY KEY (`ID_PH`,`ID`)
) DEFAULT CHARSET = cp1251;


/* Setting Of Announcements */
SET GLOBAL MAX_ALLOWED_PACKET = 4294967296;


CREATE DEFINER = 'root'@'localhost'
PROCEDURE pharm66.UpdatingOfAnnouncements(IN ID_PH INT, IN ID INT, IN Caption VARBINARY (255), IN `Text` LONGTEXT, IN Published BINARY, IN DateOfUpdating DATETIME)
BEGIN
  IF (Published = 0) THEN
    DELETE
    FROM
      Announcements
    WHERE
      ((Announcements.ID_PH = ID_PH) AND (Announcements.ID = ID));
  ELSE
    IF (EXISTS (SELECT
      *
    FROM
      Announcements AS A
    WHERE
      ((A.ID_PH = ID_PH) AND (A.ID = ID)))) THEN
      UPDATE
        Announcements AS A
      SET
        A.Caption = Caption, A.Text = Text, A.DateOfUpdating = DateOfUpdating
      WHERE
        ((A.ID_PH = ID_PH) AND (A.ID = ID));
    ELSE
      INSERT
      INTO Announcements (ID_PH, ID, Caption, Text, DateOfUpdating)
      VALUES (ID_PH, ID, Caption, Text, DateOfUpdating);
    END IF;
  END IF;
END


									/*												*/
									/* Creating Procedure Of ImportingInPriceList	*/
									/*												*/


/* Always Updated Prices */
CREATE DEFINER = 'root'@'localhost'
PROCEDURE pharm66.ImportingInPriceList(IN ID_PH INT, IN ID_PR INT, IN Price DECIMAL (12, 3), IN Preferential BINARY, IN Deleting BINARY)
BEGIN
    IF 
		(EXISTS 
		(SELECT * FROM exp_price_list AS EPL WHERE ((EPL.Id_Pharmacy = ID_PH) AND (EPL.Id_Product = ID_PR)))) 
	THEN
		UPDATE
			exp_price_list AS EPL
		SET
			EPL.Price = Price, EPL.Is_privilege = Preferential, EPL.Is_deleted = Deleting, EPL.Sent = 0
		WHERE
			((EPL.Id_Pharmacy = ID_PH) AND (EPL.Id_Product = ID_PR));
	ELSE
		INSERT
		INTO exp_price_list (Id_Pharmacy, ID_Product, Price, Is_privilege, Is_deleted, Sent)
		VALUES (ID_PH, ID_PR, Price, Preferential, Deleting, 0);
    END IF;
END


/* Updated Prices If Change */
CREATE DEFINER = 'root'@'localhost'
PROCEDURE pharm66.ImportingInPriceList(IN ID_PH INT, IN ID_PR INT, IN Price DECIMAL (12, 3), IN Preferential BINARY, IN Deleting BINARY)
BEGIN
    IF 
		(EXISTS 
		(SELECT * FROM exp_price_list AS EPL WHERE ((EPL.Id_Pharmacy = ID_PH) AND (EPL.Id_Product = ID_PR)))) 
	THEN
		IF
			(EXISTS 
			(SELECT * FROM exp_price_list AS EPL 
			WHERE ((EPL.Id_Pharmacy = ID_PH) AND (EPL.Id_Product = ID_PR) AND (EPL.Price <> Price) AND 
					(EPL.Is_privilege <> Preferential) AND (EPL.Is_deleted <> Deleting)))) 
		THEN
			UPDATE
				exp_price_list AS EPL
			SET
				EPL.Price = Price, EPL.Is_privilege = Preferential, EPL.Is_deleted = Deleting, EPL.Sent = 0
			WHERE
				((EPL.Id_Pharmacy = ID_PH) AND (EPL.Id_Product = ID_PR));
		END IF;
	ELSE
		INSERT
		INTO exp_price_list (Id_Pharmacy, ID_Product, Price, Is_privilege, Is_deleted, Sent)
		VALUES (ID_PH, ID_PR, Price, Preferential, Deleting, 0);
    END IF;
END


									/*									*/
									/* CREATING PROCEDURES OF SITE		*/
									/*									*/


									/*											*/
									/* Creating Procedure Of UpdatingDrugstores */
									/*											*/


--CREATE DEFINER = 'root'@'localhost'
CREATE PROCEDURE Pharm66.UpdatingDrugstoresOfSite(
IN ID INT, IN ID_DI INT, IN Name VARBINARY (255), IN Address VARBINARY (255), IN Phone VARBINARY (127), 
IN Mail VARBINARY (63), IN Site VARBINARY (127), IN Schedule VARBINARY (255), IN Transport VARBINARY (255), 
IN Deleting BINARY)
BEGIN
  IF (Deleting = 1) THEN
    --
    DELETE FROM
      Pharmacy
    WHERE
      (Id_Pharmacy = ID);
    --
    DELETE FROM
      Price_List
    WHERE
      (Id_Pharmacy = ID);
  ELSE
    IF
      (EXISTS (SELECT * FROM Pharmacy AS D WHERE (D.Id_Pharmacy = ID)))
    THEN
      UPDATE
        Pharmacy AS D
      SET
        D.Name_full = Name, D.Addr = Address, D.Phone = Phone, D.Mail = Mail, D.Web = Site, D.Hours = Schedule, 
        D.Trans = Transport, D.Id_District = ID_DI, D.Is_deleted = Deleting
      WHERE
        (D.Id_Pharmacy = ID);
    ELSE
      INSERT
      INTO Pharmacy (Id_Pharmacy, Name_full, Addr, Phone, Mail, Web, Hours, Trans, Id_District, Is_deleted)
      VALUES (ID, Name, Address, Phone, Mail, Site, Schedule, Transport, ID_DI, Deleting);
    END IF;
  END IF;
END


									/*											*/
									/* Creating Procedure Of UpdatingOfProducts */
									/*											*/


--CREATE DEFINER = 'root'@'localhost'
CREATE PROCEDURE Pharm66.UpdatingProductsOfSite(IN ID INT, IN ID_PG INT, IN Name VARBINARY (255), 
IN Composition VARBINARY (255), IN Updating DATETIME, IN Deleting BINARY)
BEGIN
  IF (Deleting = 1) THEN
    DELETE FROM
      Product
    WHERE
      (Id_Product = ID);
  ELSE
    IF
      (EXISTS (SELECT * FROM Product AS P WHERE (P.Id_Product = ID)))
    THEN
      UPDATE
        Product AS P
      SET
        P.Id_product_group = ID_PG, P.Name_full = Name, P.Composition = Composition, P.Date_upd = Updating, 
        P.Is_deleted = Deleting
      WHERE
        (P.Id_Product = ID);
    ELSE
      INSERT INTO Product(Id_Product, Id_product_group, Name_full, Composition, Description, Date_upd, Is_deleted)
      VALUES (ID, ID_PG, Name, Composition, '', Updating, Deleting);
    END IF;
  END IF;
END


									/*											*/
									/* Creating Procedure Of UpdatingPriceList	*/
									/*											*/


--DEFINER = 'root'@'localhost'
CREATE PROCEDURE Pharm66.UpdatingPriceListsOfSite(IN ID_PH INT, IN ID_PR INT, IN Price DECIMAL (12, 3), 
IN Updating DATETIME, IN Preferential BINARY, IN Deleting BINARY)
BEGIN
  IF (Deleting = 1) THEN
    DELETE FROM
      Price_List
    WHERE
      ((Id_Pharmacy = ID_PH) AND (Id_Product = ID_PR));
  ELSE
    IF
      (EXISTS (SELECT * FROM Price_List AS PL WHERE ((PL.Id_Pharmacy = ID_PH) AND (PL.Id_Product = ID_PR))))
    THEN
      UPDATE
        Price_list AS PL
      SET
        PL.Price = Price, PL.Date_upd = Updating, PL.Is_privilege = Preferential, PL.Is_deleted = Deleting
      WHERE
        ((PL.Id_Pharmacy = ID_PH) AND (PL.Id_Product = ID_PR));
    ELSE
      INSERT INTO Price_list (Id_Pharmacy, ID_Product, Price, Date_upd, Is_privilege, Is_deleted)
      VALUES (ID_PH, ID_PR, Price, Updating, Preferential, Deleting);
    END IF;
  END IF;
END


									/*													*/
									/* Creating Procedure Of UpdatingDatesOfPriceList	*/
									/*													*/


--CREATE DEFINER = 'root'@'localhost'
CREATE PROCEDURE Pharm66.UpdatingDatesOfPriceListOfSite(IN ID INT, IN `Date` DATETIME)
BEGIN
  UPDATE
    Price_list AS PL
  SET
    PL.Date_upd = Date
  WHERE
    (PL.Id_Pharmacy = ID);
END

