export interface SystemUser {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: "Owner" | "Admin";
}

export interface CreateAdminRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}
