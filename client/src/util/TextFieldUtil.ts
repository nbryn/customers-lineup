import StringUtil from './StringUtil';
import { TextFieldType } from '../components/TextFieldModal';

function mapKeyToLabel(key: string): string {
    if (key === 'timeSlotLength') return 'Visit Length';

    return StringUtil.capitalizeFirstLetter(key);
}

function mapKeyToType(key: string): TextFieldType {
    if (key === 'opens' || key === 'closes') return 'time';
    if (key === 'capacity' || key === 'timeSlotLength' || key === 'zip') return 'number';
    if (key === 'password') return 'password';

    return 'text';
}

export default {
    mapKeyToLabel,
    mapKeyToType,
};