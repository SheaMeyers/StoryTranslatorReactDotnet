import Button from '@mui/material/Button'
import Modal from '@mui/material/Modal'
import TextField from '@mui/material/TextField'
import Box from '@mui/material/Box'
import { useState } from 'react'
import { login } from '../../api'
import '../../styling/Modals.css'


type LoginModalProps = {
  isOpen: boolean
  handleClose: (username?: string, apiToken?: string, ) => void
}


const LoginModal = (props: LoginModalProps) => {
  const [username, setUsername] = useState<string>('')
  const [password, setPassword] = useState<string>('')
  const [error, setError] = useState<string>('')

  const handleSubmit = async () => {
    try {
      const apiToken = await login(username, password)
      props.handleClose(username, apiToken)
    } catch (e) {
      setError('Unable to login.  Please try again later')
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
        {error && <p>{error}</p>}
        <Button variant="contained" onClick={handleSubmit}>Login</Button>
        <Button variant="contained" onClick={() => props.handleClose()}>Close</Button>
      </Box>
    </Modal>
  );
}

export default LoginModal
