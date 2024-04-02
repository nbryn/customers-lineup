import React from 'react';
import {useHistory} from 'react-router-dom';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import Avatar from '@mui/material/Avatar';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import makeStyles from '@mui/styles/makeStyles';
import Link from '@mui/material/Link';

import {ComboBox, type ComboBoxOption} from '../../shared/components/form/ComboBox';
import {Form} from '../../shared/components/form/Form';
import {registerValidationSchema} from '../user/UserValidation';
import StringUtil from '../../shared/util/StringUtil';
import {TextField} from '../../shared/components/form/TextField';
import TextFieldUtil from '../../shared/util/TextFieldUtil';
import {type AddressKey, useAddress} from '../../shared/hooks/useAddress';
import {useForm} from '../../shared/hooks/useForm';
import type {Index} from '../../shared/hooks/useForm';
import {useRegisterMutation} from './AuthApi';
import type {RegisterRequest} from '../../autogenerated';
import {LOGIN_ROUTE} from '../../app/RouteConstants';
import {FormControlLabel} from '@mui/material';

const useStyles = makeStyles({
    avatar: {
        margin: 7,
    },
    box: {
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        marginTop: 65,
    },
    helperText: {
        color: 'red',
    },
    links: {
        justifyContent: 'flex-end',
        marginBottom: 50,
        marginTop: 15,
    },
    submitButton: {
        marginBottom: 2,
        marginTop: 5,
    },
});

export const RegisterView: React.FC = () => {
    const [register] = useRegisterMutation();
    const history = useHistory();
    const styles = useStyles();

    const formValues = {
        email: '',
        name: '',
        zip: 0,
        street: '',
        password: '',
    };

    const {formHandler} = useForm<typeof formValues & Index>({
        initialValues: formValues,
        validationSchema: registerValidationSchema,
        onSubmit: async (formValues) => {
            const address = addressHandler.addresses.find(
                (address) => address.street === formValues.street
            );

            await register({...formValues, address} as RegisterRequest);
        },
    });

    const addressHandler = useAddress(formHandler);
    return (
        <Container component="main" maxWidth="xs">
            <Box className={styles.box}>
                <Avatar className={styles.avatar} sx={{bgcolor: 'secondary.main'}}>
                    <LockOutlinedIcon />
                </Avatar>
                <Typography component="h1" variant="h5" marginBottom={2}>
                    {'Register'}
                </Typography>
                <Form
                    submitButtonText="Register"
                    onSubmit={formHandler.handleSubmit}
                    submitButtonStyle={styles.submitButton}
                    valid={formHandler.isValid}
                >
                    <Grid container spacing={2}>
                        {Object.keys(formValues).map((key) => {
                            if (key === 'zip' || key === 'street') {
                                return (
                                    <Grid item xs={12} key={key}>
                                        <ComboBox
                                            id={key}
                                            type="text"
                                            label={StringUtil.capitalize(key)}
                                            options={addressHandler.getLabels(key as AddressKey)}
                                            onBlur={formHandler.handleBlur}
                                            partOfForm
                                            helperText={
                                                formHandler.touched[key] && formHandler.errors[key]
                                            }
                                            defaultLabel={
                                                key === 'street' ? 'street - After Zip' : 'Zip'
                                            }
                                            setFieldValue={(option: ComboBoxOption, formFieldId) =>
                                                formHandler.setFieldValue(formFieldId, option.label)
                                            }
                                            error={
                                                formHandler.touched[key] &&
                                                Boolean(formHandler.errors[key])
                                            }
                                        />
                                    </Grid>
                                );
                            }

                            return (
                                <Grid item xs={12} key={key}>
                                    <TextField
                                        id={key}
                                        label={StringUtil.capitalize(key)}
                                        type={TextFieldUtil.mapKeyToType(key)}
                                        value={formHandler.values[key] as string}
                                        onChange={formHandler.handleChange}
                                        onBlur={formHandler.handleBlur}
                                        error={
                                            formHandler.touched[key] &&
                                            Boolean(formHandler.errors[key])
                                        }
                                        helperText={
                                            formHandler.touched[key] && formHandler.errors[key]
                                        }
                                    />
                                </Grid>
                            );
                        })}
                        <FormControlLabel
                            sx={{marginTop: 3}}
                            control={<React.Fragment />}
                            label=""
                        />
                    </Grid>
                </Form>
                <Grid container className={styles.links}>
                    <Grid item>
                        <Link onClick={() => history.push(LOGIN_ROUTE)} variant="body2" href="">
                            {'Already have an account? Login'}
                        </Link>
                    </Grid>
                </Grid>
            </Box>
            <Typography align="center" color="text.secondary" variant="body2">
                {'Copyright © Customers Lineup '}
                {new Date().getFullYear()}
                {'.'}
            </Typography>
        </Container>
    );
};
