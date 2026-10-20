export interface AuthUser {
  email: string;
  username: string;
  city: string;
  occupation: string;
}

export interface StoredUser extends AuthUser {
  password: string;
}
