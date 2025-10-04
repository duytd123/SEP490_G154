import Register from "./Register";
import { registerSeller } from "../../api/auth";

export default function RegisterSeller() {
  return <Register roleName="Người bán" api={registerSeller} />;
}
