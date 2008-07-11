INSERT INTO issue_type (
  lookup_name,
  display_name,
  description
) 
VALUES (
  'MISS', 
  'Missing Method', 
  'The application relies on a method that is not yet implemented in Mono.  The application will not be able to run with this dependency.'
);

INSERT INTO issue_type (
  lookup_name,
  display_name,
  description
) 
VALUES (
  'NIEX', 
  'NotImplementedException', 
  'The application relies on a method which exists, but will throw a NotImplementedException if called.  The application may run with this issue; however, the application will likely crash if the code that relies on this method is executed.'
);

INSERT INTO issue_type (
  lookup_name,
  display_name,
  description
) 
VALUES (
  'PINV', 
  'P/Invoke', 
  'The application relies on a non-managed library.  If the library has not been ported to the target platform, this application will not be able to run.'
);

INSERT INTO issue_type (
  lookup_name,
  display_name,
  description
) 
VALUES (
  'TODO', 
  'Method marked MonoTodo', 
  'The application relies on a method that has been implemented in Mono, but which has been marked with the [MonoTodo] attribute.  This method may exhibit unexpected behavior if the code that relies on this method is executed.'
);