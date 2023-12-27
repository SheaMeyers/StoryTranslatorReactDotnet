import AppBar from "@mui/material/AppBar"
import Button from "@mui/material/Button"
import "../styling/Header.css"
import { useState } from "react"
import SignUpModal from "./modals/SignUpModal"
import LoginModal from "./modals/LoginModal"

type HeaderProps = {
  username: string
  apiToken: string
  updateUsernameAndApiToken: (username: string, apiToken: string) => void
}

const Header = (props: HeaderProps) => {

  const [isSignUpModalOpen, setIsSignUpModalOpen] = useState<boolean>(false)
  const [isLoginModalOpen, setIsLoginModalOpen] = useState<boolean>(false)

  const handleSignUpModalClose = (username: string = "", apiToken: string = "") => {
    if (username && apiToken) props.updateUsernameAndApiToken(username, apiToken)
    setIsSignUpModalOpen(false)
  }

  const handleLoginModalClose = (username: string = "", apiToken: string = "") => {
    if (username && apiToken) props.updateUsernameAndApiToken(username, apiToken)
    setIsLoginModalOpen(false)
  }

  return (
    <>
      <AppBar position="static" className="Header">
        {props.apiToken && props.username ? (
          <>
            <Button variant="outlined" className="Header__Button">
              Logout
            </Button>
            <Button variant="outlined" className="Header__Button">
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
    </>
  )
}

export default Header
