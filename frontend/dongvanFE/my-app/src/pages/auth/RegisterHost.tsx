import Register from "./Register";
import { registerHost } from "../../api/auth";

export default function RegisterHost() {
  return <Register roleName="Chủ homestay" api={registerHost} />;
}
