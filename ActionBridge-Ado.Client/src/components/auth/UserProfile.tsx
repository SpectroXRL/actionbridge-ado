import { useMsal } from "@azure/msal-react";
import { SignOutButton } from "./SignOutButton";

export const UserProfile = () => {
  const { accounts } = useMsal();
  const account = accounts[0];

  return (
    <div className="user-profile">
      <span>Welcome, {account?.name || account?.username}</span>
      <SignOutButton />
    </div>
  );
};
