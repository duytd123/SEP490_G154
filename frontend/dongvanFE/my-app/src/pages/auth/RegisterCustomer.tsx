import Register from "./Register";
import { registerCustomer } from "../../api/auth";

export default function RegisterCustomer() {
  return <Register roleName="Khách hàng" api={registerCustomer} />;
}
