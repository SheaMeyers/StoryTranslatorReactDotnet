import Button from '@mui/material/Button'
import Modal from '@mui/material/Modal'
import TextField from '@mui/material/TextField'
import Box from '@mui/material/Box'
import { useState } from 'react'
import { signUp } from '../../api'
import '../../styling/Modals.css'


type SignUpModalProps = {
  isOpen: boolean
  handleClose: (username?: string, apiToken?: string, ) => void
}


const SignUpModal = (props: SignUpModalProps) => {
  const [username, setUsername] = useState<string>('')
  const [password, setPassword] = useState<string>('')
  const [verifyPassword, setVerifyPassword] = useState<string>('')
  const [error, setError] = useState<string>('')

  const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/

  const handleSubmit = async () => {
    if (!passwordRegex.test(password)) {
      setError('Password must contain eight characters, at least one uppercase letter, one lowercase letter and one number:')
      return
    }

    if (password !== verifyPassword) {
      setError('Passwords do not match')
      return
    }

    try {
      const apiToken = await signUp(username, password)
      props.handleClose(username, apiToken)
    } catch (e) {
      if ((e as Error).message.includes('already taken')) {
        setError((e as Error).message)
      } else {
        setError('Unable to sign up now.  Please try again later')
      }
    }
  }
  
  return (
    <Modal
      open={props.isOpen}
      onClose={() => props.handleClose()}
      aria-labelledby="modal-modal-title"
      aria-describedby="modal-modal-description"
    >
      <Box className="Modal__Box">
        <TextField
          id="username"
          label="Username"
          variant="outlined"
          required={true}
          value={username}
          onChange={(event) => setUsername(event.target.value)}
        />
        <TextField
          id="password"
          label="Password"
          variant="outlined"
          type="password"
          required={true}
          value={password}
          onChange={(event) => setPassword(event.target.value)}
        />
        <TextField
          id="verify-password"
          label="Verify Password"
          variant="outlined"
          type="password"
          required={true}
          value={verifyPassword}
          onChange={(event) => setVerifyPassword(event.target.value)}
        />
        {error && <p>{error}</p>}
        <Button variant="contained" onClick={handleSubmit}>Submit</Button>
        <Button variant="contained" onClick={() => props.handleClose()}>Close</Button>
      </Box>
    </Modal>
  );
}

export default SignUpModal
