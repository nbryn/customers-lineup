import React, {useState} from 'react';
import CircularProgress from '@mui/material/CircularProgress';
import {MenuItem} from '@mui/material';

import {ComboBox, type ComboBoxOption} from '../form/ComboBox';
import {type FormHandler} from '../../hooks/useForm';
import {Dialog} from './Dialog';
import {TextField, type TextFieldType} from '../form/TextField';
import TextFieldUtil from '../../util/TextFieldUtil';

type Props = {
    show: boolean;
    isComboBox?: boolean;
    streetOptions?: ComboBoxOption[];
    zipOptions?: ComboBoxOption[];
    textFieldKey: string;
    formHandler: FormHandler<any>;
    textFieldType: TextFieldType;
    initialValue: string | undefined;
    primaryActionText?: string;
    selectOptions?: string[];
    primaryAction?: () => Promise<void>;
    showModal: (value: string) => void;
};

export const TextFieldModal: React.FC<Props> = ({
    show,
    isComboBox = false,
    streetOptions,
    zipOptions,
    textFieldKey,
    initialValue,
    primaryAction,
    primaryActionText,
    textFieldType,
    formHandler,
    selectOptions,
    showModal,
}: Props) => {
    const [updating, setUpdating] = useState(false);

    return (
        <Dialog
            open={show}
            title={`Update ${TextFieldUtil.mapKeyToLabel(textFieldKey, true)}`}
            primaryActionText={primaryActionText}
            primaryDisabled={!formHandler.isValid}
            primaryAction={async () => {
                setUpdating(true);
                await primaryAction!();

                setUpdating(false);
            }}
            secondaryAction={() => {
                formHandler.setFieldValue(textFieldKey, initialValue);
                showModal('');
            }}
        >
            {updating && <CircularProgress />}
            {isComboBox ? (
                <>
                    {['zip', 'street'].map((entry) => {
                        return (
                            <ComboBox
                                id={entry}
                                style={{width: '100%', marginLeft: 0}}
                                label={TextFieldUtil.mapKeyToLabel(entry)}
                                type="text"
                                options={(entry === 'zip' ? zipOptions : streetOptions) ?? []}
                                onBlur={formHandler.handleBlur}
                                setFieldValue={(option: ComboBoxOption, formFieldId: string) =>
                                    formHandler.setFieldValue(formFieldId, option.label)
                                }
                                error={formHandler.touched[entry] && !!formHandler.errors[entry]}
                                helperText={
                                    formHandler.touched[entry] && (formHandler.errors[entry] as any)
                                }
                                defaultLabel={entry === 'street' ? 'Zip before street' : ''}
                            />
                        );
                    })}
                </>
            ) : (
                <TextField
                    id={textFieldKey}
                    label={TextFieldUtil.mapKeyToLabel(textFieldKey)}
                    type={textFieldType}
                    value={formHandler.values[textFieldKey] || ''}
                    onChange={formHandler.handleChange(textFieldKey)}
                    onBlur={formHandler.handleBlur}
                    error={formHandler.touched[textFieldKey] && !!formHandler.errors[textFieldKey]}
                    helperText={
                        formHandler.touched[textFieldKey] &&
                        (formHandler.errors[textFieldKey] as any)
                    }
                    select={textFieldKey === 'type'}
                    inputLabelProps={{
                        shrink:
                            textFieldKey === 'opens' || textFieldKey === 'closes'
                                ? true
                                : undefined,
                    }}
                    step={TextFieldUtil.mapKeyToStep(textFieldKey)}
                >
                    {selectOptions &&
                        selectOptions.map((x, index) => (
                            <MenuItem key={index} value={x}>
                                {x}
                            </MenuItem>
                        ))}
                </TextField>
            )}
        </Dialog>
    );
};
