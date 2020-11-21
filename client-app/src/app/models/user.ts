//What we get from the DB
export interface IUser {
  username: string;
  displayName: string;
  token: string;
  image?: string;
}

//What we send to the DB
export interface IUserFormValues {
  email: string;
  password: string;
  displayName?: string;
  username?: string;
}
