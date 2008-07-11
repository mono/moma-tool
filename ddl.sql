CREATE DATABASE nhibernate WITH OWNER=dick ENCODING='UNICODE';
\connect nhibernate
CREATE PROCEDURAL LANGUAGE plpgsql;

DROP TABLE issue;
DROP TABLE report;
DROP TABLE moma_definition;
DROP TABLE issue_type;

DROP FUNCTION CreateOrUpdateReport (
  p_moma_definition_id integer,
  p_report_filename character varying(50),
  p_report_date timestamp without time zone,
  p_reporter_ip character varying(50),
  p_reporter_name character varying(500),
  p_reporter_email character varying(500),
  p_reporter_organization character varying(500),
  p_reporter_homepage character varying(500),
  p_reporter_comments text
);

DROP FUNCTION GetOrCreateDefinition (p_lookup varchar(100));

DROP FUNCTION CreateIssue (
    p_report_id integer,
    p_issue_type_id integer,
    p_method_return_type character varying(200),
    p_method_namespace character varying(200),
    p_method_class character varying(200),
    p_method_name character varying(1000),
    p_method_library character varying(500)
);

-- Table: issue_type

CREATE TABLE issue_type
(
  id serial NOT NULL,
  lookup_name character varying(10) NOT NULL,
  display_name character varying(100) NOT NULL,
  description text NOT NULL,
  is_active boolean NOT NULL DEFAULT true,
  CONSTRAINT pk_issue_type PRIMARY KEY (id)
);

-- Table: moma_definition


CREATE TABLE moma_definition
(
  id serial NOT NULL,
  lookup_name character varying(100) NOT NULL,
  display_name character varying(100) NOT NULL,
  description text NOT NULL,
  create_date timestamp without time zone NOT NULL DEFAULT ('now'::text)::timestamp(3) with time zone,
  is_active boolean NOT NULL DEFAULT true,
  CONSTRAINT pk_moma_definition PRIMARY KEY (id)
);


CREATE TABLE report
(
  id serial NOT NULL,
  moma_definition_id integer,
  report_filename character varying(50) NOT NULL,
  report_date timestamp without time zone NOT NULL DEFAULT ('now'::text)::timestamp(3) with time zone,
  reporter_ip character varying(50),
  reporter_name character varying(500),
  reporter_email character varying(500),
  reporter_organization character varying(500),
  reporter_homepage character varying(500),
  reporter_comments text,
  create_date timestamp without time zone NOT NULL DEFAULT ('now'::text)::timestamp(3) with time zone,
  last_update_date timestamp without time zone NOT NULL DEFAULT ('now'::text)::timestamp(3) with time zone,
  is_active boolean NOT NULL DEFAULT true,
  CONSTRAINT pk_report PRIMARY KEY (id),
  CONSTRAINT fk_report_moma_definition FOREIGN KEY (moma_definition_id)
      REFERENCES moma_definition (id) MATCH SIMPLE
      ON UPDATE CASCADE ON DELETE CASCADE
);

-- Table: report

-- Table: issue

CREATE TABLE issue
(
  id serial NOT NULL,
  report_id integer NOT NULL,
  issue_type_id integer NOT NULL,
  method_return_type character varying(200),
  method_namespace character varying(200),
  method_class character varying(200),
  method_name character varying(1000),
  method_library character varying(500),
  CONSTRAINT pk_issue PRIMARY KEY (id),
  CONSTRAINT fk_issue_issue_type FOREIGN KEY (issue_type_id)
      REFERENCES issue_type (id) MATCH SIMPLE
      ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT fk_issue_report FOREIGN KEY (report_id)
      REFERENCES report (id) MATCH SIMPLE
      ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE FUNCTION GetOrCreateDefinition (p_lookup varchar(100)) RETURNS SETOF int AS $$
DECLARE
        r RECORD;
BEGIN
    
  IF NOT EXISTS (SELECT id FROM moma_definition WHERE lookup_name ILIKE p_lookup LIMIT 1) THEN
    INSERT INTO moma_definition(lookup_name, display_name, description)
      VALUES (p_lookup, p_lookup, p_lookup);
  END IF;
  FOR r IN SELECT id FROM moma_definition WHERE lookup_name ILIKE p_lookup LIMIT 1 LOOP
    RETURN NEXT r.id;
  END LOOP;
  RETURN;
END;
$$ LANGUAGE plpgsql;

CREATE FUNCTION CreateOrUpdateReport (
  p_moma_definition_id integer,
  p_report_filename character varying(50),
  p_report_date timestamp without time zone,
  p_reporter_ip character varying(50),
  p_reporter_name character varying(500),
  p_reporter_email character varying(500),
  p_reporter_organization character varying(500),
  p_reporter_homepage character varying(500),
  p_reporter_comments text
) RETURNS SETOF int AS $$
DECLARE
        r RECORD;
BEGIN
    
  IF EXISTS (SELECT id FROM report WHERE report_filename ILIKE p_report_filename LIMIT 1) THEN
    UPDATE report
    SET 
        moma_definition_id=p_moma_definition_id, 
        report_filename=p_report_filename, 
        report_date=p_report_date, 
        reporter_ip=p_reporter_ip, 
        reporter_name=p_reporter_name, 
        reporter_email=p_reporter_email, 
        reporter_organization=p_reporter_organization, 
        reporter_homepage=p_reporter_homepage, 
        reporter_comments=p_reporter_comments,
        last_update_date=('now'::text)::timestamp(3)
    WHERE report_filename=p_report_filename;
  ELSE
    INSERT INTO report(
        moma_definition_id, 
        report_filename, 
        report_date, 
        reporter_ip, 
        reporter_name, 
        reporter_email, 
        reporter_organization, 
        reporter_homepage, 
        reporter_comments)
    VALUES (
        p_moma_definition_id, 
        p_report_filename, 
        p_report_date, 
        p_reporter_ip, 
        p_reporter_name, 
        p_reporter_email, 
        p_reporter_organization, 
        p_reporter_homepage, 
        p_reporter_comments);

    DELETE FROM issue
    WHERE id = (SELECT id FROM report WHERE report_filename ILIKE p_report_filename LIMIT 1);

  END IF;
  FOR r IN SELECT id FROM report WHERE report_filename ILIKE p_report_filename LIMIT 1 LOOP
    RETURN NEXT r.id;
  END LOOP;
  RETURN;
END;
$$ LANGUAGE plpgsql;

CREATE FUNCTION CreateIssue (
    p_report_id integer,
    p_issue_type_id integer,
    p_method_return_type character varying(200),
    p_method_namespace character varying(200),
    p_method_class character varying(200),
    p_method_name character varying(1000),
    p_method_library character varying(500)
) RETURNS SETOF int AS $$
DECLARE
        pId int;
        r RECORD;
BEGIN
  INSERT INTO issue (
    report_id, 
    issue_type_id, 
    method_return_type, 
    method_namespace, 
    method_class, 
    method_name, 
    method_library
  )
  VALUES (
    p_report_id, 
    p_issue_type_id, 
    p_method_return_type, 
    p_method_namespace, 
    p_method_class, 
    p_method_name, 
    p_method_library
  );
  pId = currval('issue_id_seq');
  FOR r IN SELECT pId AS id LOOP
    RETURN NEXT r.id;
  END LOOP;
  RETURN;
END;
$$ LANGUAGE plpgsql;

