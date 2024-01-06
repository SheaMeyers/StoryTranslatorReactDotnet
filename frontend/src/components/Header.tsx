import AppBar from "@mui/material/AppBar"
import Button from "@mui/material/Button"
import { useState } from "react"
import SignUpModal from "./modals/SignUpModal"
import LoginModal from "./modals/LoginModal"
import LogoutModal from "./modals/LogoutModal"
import ChangePasswordModal from "./modals/ChangePasswordModal"
import "../styling/Header.css"


type HeaderProps = {
  username: string
  apiToken: string
  updateUsernameAndApiToken: (username: string, apiToken: string) => void
}

const Header = (props: HeaderProps) => {

  const [isSignUpModalOpen, setIsSignUpModalOpen] = useState<boolean>(false)
  const [isLoginModalOpen, setIsLoginModalOpen] = useState<boolean>(false)
  const [isLogoutModalOpen, setIsLogoutModalOpen] = useState<boolean>(false)
  const [isChangePasswordModalOpen, setIsChangePasswordModalOpen] = useState<boolean>(false)

  const handleSignUpModalClose = (username: string = "", apiToken: string = "") => {
    if (username && apiToken) props.updateUsernameAndApiToken(username, apiToken)
    setIsSignUpModalOpen(false)
  }

  const handleLoginModalClose = (username: string = "", apiToken: string = "") => {
    if (username && apiToken) props.updateUsernameAndApiToken(username, apiToken)
    setIsLoginModalOpen(false)
  }

  const handleLogoutModalClose = (loggedOut: boolean = false) => {
    if (loggedOut) props.updateUsernameAndApiToken('', '')
    setIsLogoutModalOpen(false)
  }

  const handleUpdateApiToken = (apiToken: string): void => props.updateUsernameAndApiToken(props.username, apiToken)

  const handleChangePasswordModalClose = () => setIsChangePasswordModalOpen(false)

  return (
    <>
      <AppBar position="static" className="Header">
        {props.apiToken && props.username ? (
          <>
            <Button variant="outlined" className="Header__Button" onClick={() => setIsLogoutModalOpen(true)}>
              Logout
            </Button>
            <Button variant="outlined" className="Header__Button" onClick={() => setIsChangePasswordModalOpen(true)}>
              Change Password
            </Button>
            <p className="Header__Paragraph">Hello {props.username}</p>
          </>
        ) : (
          <>
            <Button variant="outlined" className="Header__Button" onClick={() => setIsLoginModalOpen(true)}>
              Login
            </Button>
            <Button variant="outlined" className="Header__Button" onClick={() => setIsSignUpModalOpen(true)}>
              Sign Up
            </Button>
          </>
        )}
      </AppBar>
      <SignUpModal isOpen={isSignUpModalOpen} handleClose={handleSignUpModalClose} />
      <LoginModal isOpen={isLoginModalOpen} handleClose={handleLoginModalClose} />
      <LogoutModal isOpen={isLogoutModalOpen} handleClose={handleLogoutModalClose} apiToken={props.apiToken} />
      <ChangePasswordModal isOpen={isChangePasswordModalOpen} handleClose={handleChangePasswordModalClose} apiToken={props.apiToken} handleUpdateApiToken={handleUpdateApiToken}/>
    </>
  )
}

export default Header
