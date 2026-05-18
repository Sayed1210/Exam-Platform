// import { redirect } from "next/navigation";

// export default function Home() {
//   redirect("/login");
// }
import { redirect } from "next/navigation";
import { cookies } from "next/headers";

export default async function Home() {
  const token = (await cookies()).get("token")?.value;

  if (token) {
    redirect("/candidates");
  }

  redirect("/login");
}