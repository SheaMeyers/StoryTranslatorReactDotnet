import Button from '@mui/material/Button'
import Modal from '@mui/material/Modal'
import TextField from '@mui/material/TextField'
import Box from '@mui/material/Box'
import { useState } from 'react'
import { changePassword } from '../../UserApi'
import '../../styling/Modals.css'


type ChangePasswordModalProps = {
  isOpen: boolean
  apiToken: string
  handleClose: () => void
}


const ChangePasswordModal = (props: ChangePasswordModalProps) => {
  const [oldPassword, setOldPassword] = useState<string>('')
  const [newPassword, setNewPassword] = useState<string>('')
  const [verifyNewPassword, setVerifyNewPassword] = useState<string>('')
  const [feedback, setFeedback] = useState<string>('')

  const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/

  const handleSubmit = async () => {
    if (!passwordRegex.test(newPassword)) {
      setFeedback('Password must contain eight characters, at least one uppercase letter, one lowercase letter and one number:')
      return
    }

    if (newPassword !== verifyNewPassword) {
      setFeedback('Passwords do not match')
      return
    }

    try {
      const isSuccessful = await changePassword(props.apiToken, oldPassword, newPassword)
      isSuccessful ? setFeedback('Success!') : setFeedback('Unable to change password.  Please try again')
    } catch (e) {
      setFeedback('Unable to change password.  Please try again')
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
          id="old-password"
          label="Old Password"
          variant="outlined"
          type="password"
          required={true}
          value={oldPassword}
          onChange={(event) => setOldPassword(event.target.value)}
        />
        <TextField
          id="password"
          label="Password"
          variant="outlined"
          type="password"
          required={true}
          value={newPassword}
          onChange={(event) => setNewPassword(event.target.value)}
        />
        <TextField
          id="verify-password"
          label="Verify Password"
          variant="outlined"
          type="password"
          required={true}
          value={verifyNewPassword}
          onChange={(event) => setVerifyNewPassword(event.target.value)}
        />
        {feedback && <p>{feedback}</p>}
        <Button variant="contained" onClick={handleSubmit}>Submit</Button>
        <Button variant="contained" onClick={() => props.handleClose()}>Close</Button>
      </Box>
    </Modal>
  );
}

export default ChangePasswordModal
